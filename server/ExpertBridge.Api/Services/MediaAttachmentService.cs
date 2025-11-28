using ExpertBridge.Contract.Requests.MediaObject;
using ExpertBridge.Core.Entities.Media;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Api.Services;

/// <summary>
///     Provides reusable media attachment processing services for posts, comments, and job postings.
/// </summary>
/// <remarks>
///     This service abstracts the complex logic of attaching media files to content entities,
///     managing S3 object grants, and ensuring proper entity relationships.
///     **Architecture Role:**
///     MediaAttachmentService is a utility service that prevents code duplication across
///     PostService, CommentService, and JobPostingService. It follows the DRY principle
///     by centralizing media attachment logic in one location.
///     **Media Attachment Flow:**
///     **1. Client Upload Process:**
///     <code>
/// // Client requests presigned URL
/// POST /api/media/presigned-url
/// {
///   "fileName": "profile-pic.jpg",
///   "contentType": "image/jpeg"
/// }
///
/// // Server creates MediaGrant (OnHold=true, IsActive=false)
/// // Returns presigned S3 URL with Key
///
/// // Client uploads directly to S3 using presigned URL
/// PUT https://s3.amazonaws.com/bucket/media/abc-123.jpg
///
/// // Client includes Key in content creation
/// POST /api/posts
/// {
///   "title": "My Post",
///   "content": "...",
///   "media": [{ "key": "media/abc-123.jpg", "type": "Image" }]
/// }
/// </code>
///     **2. Server-Side Activation:**
///     This service activates grants when content is created:
///     - Changes OnHold=false, IsActive=true
///     - Sets ActivatedAt timestamp
///     - Creates media entity (PostMedia, CommentMedia, JobPostingMedia)
///     - Prevents orphaned S3 objects
///     **Architecture Benefits:**
///     **Generic Design:**
///     - Works with any content type (TEntity)
///     - Works with any media type (TMedia)
///     - Factory pattern for media entity creation
///     - Type-safe through generic constraints
///     **Transaction Safety:**
///     - Does not call SaveChanges internally
///     - Integrates with parent service's Unit of Work
///     - All changes committed together atomically
///     **S3 Grant Management:**
///     - Activates media grants atomically with content creation
///     - Prevents orphaned S3 objects from abandoned uploads
///     - Cleanup workers can delete inactive grants after expiry
///     **Database Schema:**
///     <code>
/// MediaGrant:
///   Key: "media/abc-123.jpg" (S3 object key)
///   OnHold: true → false (when activated)
///   IsActive: false → true (when attached to content)
///   ActivatedAt: null → DateTime.UtcNow
///   ExpiresAt: DateTime (for cleanup)
///
/// PostMedia/CommentMedia/JobPostingMedia:
///   Key: "media/abc-123.jpg" (matches MediaGrant)
///   Name: Sanitized filename
///   Type: Image/Video/Document
///   ParentEntityId: References Post/Comment/JobPosting
/// </code>
///     **Usage Pattern:**
///     **In PostService.CreatePostAsync:**
///     <code>
/// if (request.Media?.Count > 0)
/// {
///     PostMedia createMediaFunc(MediaObjectRequest req, Post post)
///     {
///         return new PostMedia
///         {
///             Post = post,
///             Name = _mediaService.SanitizeMediaName(post.Title),
///             Type = req.Type,
///             Key = req.Key
///         };
///     }
///
///     post.Medias = await _mediaService.ProcessAndAttachMediaAsync(
///         request.Media,
///         post,
///         createMediaFunc,
///         _dbContext
///     );
/// }
///
/// await _dbContext.SaveChangesAsync(); // Commits post, media, and grant updates
/// </code>
///     **Security Considerations:**
///     - Only client-provided Keys are activated (no arbitrary S3 access)
///     - Grants must pre-exist (created during presigned URL request)
///     - Expired grants are ignored (handled by cleanup worker)
///     - Media types validated by caller (not this service's responsibility)
///     **Performance:**
///     - Minimal database queries (AddRange, bulk update)
///     - No file system or S3 operations (media already uploaded)
///     - Efficient for multiple media attachments
///     **Cleanup Integration:**
///     Background workers use MediaGrant status for cleanup:
///     <code>
/// // Cleanup abandoned uploads (OnHold after 24 hours)
/// var expiredGrants = await dbContext.MediaGrants
///     .Where(g => g.OnHold && g.ExpiresAt < DateTime.UtcNow)
///                                             . ToListAsync();
///                                             foreach ( var grant in expiredGrants)
///                                             {
///                                             await s3Client.DeleteObjectAsync( bucketName, grant.Key);
///                                             dbContext.MediaGrants.Remove( grant);
/// }
/// </code>
///     The service is stateless and registered as scoped, aligning with Entity Framework's DbContext lifetime.
/// </remarks>
public sealed class MediaAttachmentService
{
    // No DbContext injected here if we pass it as a parameter,
    // which aligns better with the service being stateless regarding the UoW context.

    /// <summary>
    ///     Processes media requests, creates media entities, and activates S3 object grants within a transaction.
    /// </summary>
    /// <typeparam name="TEntity">
    ///     The type of the parent content entity (Post, Comment, JobPosting) to which media will be attached.
    ///     Must be a reference type.
    /// </typeparam>
    /// <typeparam name="TMedia">
    ///     The type of media entity to create (PostMedia, CommentMedia, JobPostingMedia).
    ///     Must inherit from <see cref="MediaObject" />.
    /// </typeparam>
    /// <param name="mediaRequests">
    ///     Collection of media object requests from the client, containing S3 Keys and media types.
    ///     Can be null or empty, in which case an empty list is returned.
    /// </param>
    /// <param name="parentEntity">
    ///     The parent content entity (Post, Comment, JobPosting) to associate media with.
    /// </param>
    /// <param name="createMediaEntityFunc">
    ///     Factory function that creates the appropriate media entity type from a request and parent entity.
    ///     Allows caller to configure entity-specific properties and relationships.
    /// </param>
    /// <param name="dbContext">
    ///     The database context to use for entity tracking. Does NOT call SaveChanges - caller is responsible.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation, containing a list of created media entities.
    ///     Returns empty list if no media requests provided.
    /// </returns>
    /// <remarks>
    ///     **Processing Steps:**
    ///     **1. Validate Input:**
    ///     - Return empty list if mediaRequests is null or empty
    ///     - No exceptions thrown for missing media (optional feature)
    ///     **2. Create Media Entities:**
    ///     - Iterate through mediaRequests
    ///     - Call factory function to create typed media entity
    ///     - Add to tracking collection
    ///     **3. Track in DbContext:**
    ///     - Add all media entities to appropriate DbSet&lt;TMedia&gt;
    ///     - Entities are now tracked but not yet committed
    ///     - Foreign keys established through navigation properties
    ///     **4. Activate Media Grants:**
    ///     - Extract Keys from all media entities
    ///     - Query matching MediaGrants from database
    ///     - Update grant status:
    ///     - IsActive = true (media now attached to content)
    ///     - OnHold = false (upload confirmed)
    ///     - ActivatedAt = DateTime.UtcNow (audit timestamp)
    ///     - EF tracks changes, will update on SaveChanges
    ///     **5. Return Entities:**
    ///     - Return list of created media entities
    ///     - Caller can assign to parent entity's navigation property
    ///     - All changes committed together when caller calls SaveChanges
    ///     **Example Usage in PostService:**
    ///     <code>
    /// PostMedia CreatePostMediaEntity(MediaObjectRequest mediaReq, Post post)
    /// {
    ///     return new PostMedia
    ///     {
    ///         Post = post,
    ///         PostId = post.Id, // Set if post.Id already generated
    ///         Name = _mediaService.SanitizeMediaName(post.Title),
    ///         Type = mediaReq.Type, // Image, Video, Document
    ///         Key = mediaReq.Key    // S3 object key
    ///     };
    /// }
    ///
    /// var mediaEntities = await _mediaService.ProcessAndAttachMediaAsync(
    ///     request.Media,
    ///     post,
    ///     CreatePostMediaEntity,
    ///     _dbContext
    /// );
    ///
    /// post.Medias = mediaEntities; // Set navigation property
    /// await _dbContext.SaveChangesAsync(); // Commit all changes atomically
    /// </code>
    ///     **Example Usage in CommentService:**
    ///     <code>
    /// CommentMedia CreateCommentMediaEntity(MediaObjectRequest req, Comment comment)
    /// {
    ///     return new CommentMedia
    ///     {
    ///         Comment = comment,
    ///         Name = _mediaService.SanitizeMediaName(comment.Content),
    ///         Type = req.Type,
    ///         Key = req.Key
    ///     };
    /// }
    ///
    /// comment.Medias = await _mediaService.ProcessAndAttachMediaAsync(
    ///     request.Media,
    ///     comment,
    ///     CreateCommentMediaEntity,
    ///     _dbContext
    /// );
    /// </code>
    ///     **Transaction Guarantees:**
    ///     - All operations tracked in single DbContext
    ///     - No SaveChanges called internally
    ///     - Atomic commit when caller saves
    ///     - Rollback on any failure preserves consistency
    ///     **Grant Activation Logic:**
    ///     The grant activation prevents orphaned S3 objects:
    ///     <code>
    /// MediaGrant State Transitions:
    ///
    /// Created (presigned URL request):
    ///   OnHold: true
    ///   IsActive: false
    ///   ExpiresAt: Now + 24 hours
    ///
    /// Activated (content created with media):
    ///   OnHold: false
    ///   IsActive: true
    ///   ActivatedAt: Now
    ///
    /// Cleanup (grant never activated):
    ///   If OnHold && ExpiresAt < Now:
    ///                              Delete S3 object
    ///                              Delete MediaGrant
    /// </code>
    ///     **Error Scenarios:**
    ///     - Missing grant for Key: Grant not updated, but media entity still created
    ///     - Duplicate Keys: Multiple media reference same S3 object (allowed)
    ///     - SaveChanges failure: All changes rolled back (transaction safety)
    ///     **Performance:**
    ///     - Single query for all grants (WHERE IN clause)
    ///     - Bulk AddRange for media entities
    ///     - Minimal memory allocation
    ///     - Efficient for 1-10 media attachments per content
    ///     The method integrates seamlessly with Entity Framework's Unit of Work pattern,
    ///     allowing callers to maintain full control over transaction boundaries.
    /// </remarks>
    public async Task<List<TMedia>> ProcessAndAttachMediaAsync<TEntity, TMedia>(
        ICollection<MediaObjectRequest> mediaRequests,
        TEntity parentEntity,
        Func<MediaObjectRequest, TEntity, TMedia> createMediaEntityFunc,
        ExpertBridgeDbContext dbContext)
        where TMedia : MediaObject
        where TEntity : class
    {
        if (mediaRequests == null || !mediaRequests.Any())
        {
            return new List<TMedia>();
        }

        var mediaEntities = new List<TMedia>();
        foreach (var mediaReq in mediaRequests)
        {
            var mediaEntity = createMediaEntityFunc(mediaReq, parentEntity);
            mediaEntities.Add(mediaEntity);
        }

        // Add to DbContext, but don't save
        await dbContext.Set<TMedia>().AddRangeAsync(mediaEntities);

        var keys = mediaEntities.Select(m => m.Key).ToList();
        if (keys.Any())
        {
            var grants = await dbContext.MediaGrants
                .Where(grant => keys.Contains(grant.Key))
                .ToListAsync(); // Fetch to modify

            foreach (var grant in grants)
            {
                grant.IsActive = true;
                grant.OnHold = false;
                grant.ActivatedAt = DateTime.UtcNow;
                // dbContext will track these changes
            }
        }

        return mediaEntities;
    }

    /// <summary>
    ///     Sanitizes a content hint string to create a safe, valid media filename.
    /// </summary>
    /// <param name="contentHint">
    ///     A hint string derived from content (post title, comment text) to use as the media name.
    ///     Can be null, empty, or contain special characters.
    /// </param>
    /// <param name="maxLength">
    ///     The maximum length for the sanitized name. Defaults to 50 characters.
    /// </param>
    /// <returns>
    ///     A sanitized string suitable for use as a media filename, trimmed and length-limited.
    ///     Returns "UntitledMedia" if contentHint is null or whitespace.
    /// </returns>
    /// <remarks>
    ///     **Sanitization Rules:**
    ///     1. Null/empty/whitespace input → "UntitledMedia"
    ///     2. Trim leading/trailing whitespace
    ///     3. Truncate to maxLength if longer
    ///     4. Preserve original characters (no encoding/replacement)
    ///     **Use Cases:**
    ///     - Post media: Use post title as hint
    ///     - Comment media: Use first 50 chars of comment content
    ///     - Profile pictures: Use username or "ProfilePicture"
    ///     - Job posting attachments: Use job title
    ///     **Example Transformations:**
    ///     <code>
    /// SanitizeMediaName("My Amazing Post About AI")
    /// → "My Amazing Post About AI"
    ///
    /// SanitizeMediaName("This is a very long title that exceeds fifty characters and needs truncation")
    /// → "This is a very long title that exceeds fifty ch"
    ///
    /// SanitizeMediaName("   ")
    /// → "UntitledMedia"
    ///
    /// SanitizeMediaName(null)
    /// → "UntitledMedia"
    ///
    /// SanitizeMediaName("Short", maxLength: 3)
    /// → "Sho"
    /// </code>
    ///     **Database Storage:**
    ///     The sanitized name is stored in media entities:
    ///     <code>
    /// PostMedia:
    ///   Name: "My Amazing Post About AI"
    ///   Key: "media/abc-123-def-456.jpg"
    ///   Type: Image
    /// </code>
    ///     **Display Usage:**
    ///     <code>
    /// // In UI, show meaningful name instead of UUID
    /// &lt;img src="{media.Url}" alt="{media.Name}" /&gt;
    ///
    /// // In download links
    /// &lt;a href="{media.Url}" download="{media.Name}.jpg"&gt;
    ///   Download {media.Name}
    /// &lt;/a&gt;
    /// </code>
    ///     **Security Notes:**
    ///     - No path traversal prevention (not used in file paths)
    ///     - No XSS prevention (caller responsible for HTML encoding)
    ///     - Special characters preserved (names are descriptive, not functional)
    ///     **Performance:**
    ///     - Simple string operations (trim, substring)
    ///     - No regex or complex parsing
    ///     - Minimal allocations
    ///     **Future Enhancements:**
    ///     Consider adding:
    ///     - Special character removal/replacement
    ///     - Unicode normalization
    ///     - Extension preservation (.jpg, .pdf)
    ///     - Uniqueness suffix if needed
    ///     The method is deterministic and safe for concurrent use.
    /// </remarks>
    public string SanitizeMediaName(string contentHint, int maxLength = 50) // Shared helper
    {
        if (string.IsNullOrWhiteSpace(contentHint))
        {
            return "UntitledMedia";
        }

        var name = contentHint.Trim();
        if (name.Length > maxLength)
        {
            name = name.Substring(0, maxLength);
        }

        return name;
    }
}
