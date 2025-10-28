using ExpertBridge.Core.Entities.Comments;
using ExpertBridge.Core.Entities.CommentVotes;
using ExpertBridge.Core.Entities.Media.CommentMedia;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Exceptions;
using ExpertBridge.Contract.Messages;
using ExpertBridge.Contract.Queries;
using ExpertBridge.Contract.Requests.CreateComment;
using ExpertBridge.Contract.Requests.EditComment;
using ExpertBridge.Contract.Requests.MediaObject;
using ExpertBridge.Contract.Responses;
using ExpertBridge.Data.DatabaseContexts;
using ExpertBridge.Notifications;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ExpertBridge.Application.DomainServices;

/// <summary>
///     Provides comprehensive comment management for posts and job postings with support for nested replies,
///     media attachments, voting, and content moderation.
/// </summary>
/// <remarks>
///     This service manages the complete lifecycle of comments in the ExpertBridge social engagement system,
///     including creation with media, retrieval with various filters, editing with authorization, voting mechanics,
///     and deletion with cascade handling.
///     **Core Features:**
///     - Nested comment threads (replies via ParentCommentId)
///     - Multi-media attachment support (images, videos)
///     - Upvote/downvote with toggle logic
///     - Tag inheritance from parent posts for interest tracking
///     - Content moderation pipeline integration
///     - Real-time notifications via SignalR
///     - Authorization checks for edit/delete
///     **Comment Architecture:**
///     <code>
/// Post/JobPosting
///     ↓
/// Comment (level 1)
///     ↓
/// Comment (level 2 - reply via ParentCommentId)
///     ↓
/// Comment (level 3 - nested reply)
/// </code>
///     **Tag Inheritance Flow:**
///     <code>
/// Post has tags: ["csharp", "azure"]
///     ↓
/// User creates comment on post
///     ↓
/// Post tags added to commenter's UserInterests
///     ↓
/// UserInterestsUpdatedMessage published
///     ↓
/// Commenter's embedding vector updated
///     ↓
/// Personalized recommendations improved
/// </code>
///     **Voting System:**
///     Toggle behavior prevents spam and allows vote changes:
///     <code>
/// User upvotes → CommentVote created (IsUpvote=true)
/// User upvotes again → CommentVote deleted (vote removed)
/// User downvotes → CommentVote updated (IsUpvote=false)
/// User downvotes again → CommentVote deleted (vote removed)
/// </code>
///     **Content Moderation:**
///     After comment creation, DetectInappropriateCommentMessage published:
///     - Background worker analyzes content with AI
///     - Flags inappropriate comments
///     - Sends notifications to moderators
///     - Potential auto-hide based on toxicity score
///     **Media Handling:**
///     - Supports multiple attachments per comment
///     - S3 storage with presigned URLs
///     - Media grant activation (15-day expiration)
///     - Filenames sanitized
///     - Processed via MediaAttachmentService
///     **Notification Integration:**
///     Automatic notifications sent for:
///     - New comment on user's post (to post author)
///     - Reply to user's comment (to parent comment author)
///     - Upvote on user's comment (to comment author)
///     - Real-time delivery via SignalR
///     **Authorization Rules:**
///     - Edit: Only comment author can edit their own comments
///     - Delete: Only comment author can delete their own comments
///     - Vote: Users cannot vote on their own comments
///     - View: Public (all comments visible to all users)
///     **Query Patterns:**
///     - GetCommentAsync: Single comment with author, media, vote counts
///     - GetCommentsByPostAsync: All comments for a post (ordered by creation)
///     - GetCommentsByJobAsync: All comments for a job posting
///     - GetCommentsByProfileAsync: User's comment history across all content
///     **Performance Optimization:**
///     - Eager loading: Include(Author).Include(CommentMedia)
///     - AsNoTracking for read-only queries
///     - Selective projection to CommentResponse DTOs
///     - Vote count aggregation in query
///     **Use Cases:**
///     - Threaded discussions on posts
///     - Job posting Q&A
///     - Expert feedback and advice
///     - Community engagement tracking
///     - Content moderation workflow
///     - User reputation building (via comment quality)
///     Registered as scoped service for per-request lifetime and DbContext transaction management.
/// </remarks>
public class CommentService
{
    private readonly IValidator<CreateCommentRequest> _createCommentValidator;
    private readonly ExpertBridgeDbContext _dbContext;
    private readonly IValidator<EditCommentRequest> _editCommentValidator;
    private readonly ILogger<CommentService> _logger;
    private readonly MediaAttachmentService _mediaService;
    private readonly NotificationFacade _notificationFacade;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly TaggingService _taggingService;

    public CommentService(
        ExpertBridgeDbContext dbContext,
        MediaAttachmentService mediaService,
        NotificationFacade notificationFacade,
        TaggingService taggingService,
        ILogger<CommentService> logger,
        IPublishEndpoint publishEndpoint,
        IValidator<CreateCommentRequest> createCommentValidator,
        IValidator<EditCommentRequest> editCommentValidator)
    {
        _dbContext = dbContext;
        _mediaService = mediaService;
        _notificationFacade = notificationFacade;
        _taggingService = taggingService;
        _logger = logger;
        _publishEndpoint = publishEndpoint;
        _createCommentValidator = createCommentValidator;
        _editCommentValidator = editCommentValidator;
    }

    /// <summary>
    ///     Creates a new comment on a post or job posting with optional media attachments and nested reply support.
    /// </summary>
    /// <param name="request">The comment creation request containing content, target post/job, parent comment, and media.</param>
    /// <param name="authorProfile">The authenticated user profile creating the comment.</param>
    /// <returns>A task containing the created comment with populated metadata.</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null.</exception>
    /// <exception cref="ArgumentException">Thrown when Content is null or empty.</exception>
    /// <exception cref="PostNotFoundException">Thrown when neither PostId nor JobPostingId resolves to an existing entity.</exception>
    /// <exception cref="CommentNotFoundException">Thrown when ParentCommentId doesn't resolve to an existing comment.</exception>
    /// <remarks>
    ///     **Complete Creation Workflow:**
    ///     **1. Target Validation:**
    ///     - Validates that either Post (PostId) OR JobPosting (JobPostingId) exists
    ///     - Comments must target exactly one entity type
    ///     - Eagerly loads tags from target (PostTags/JobPostingTags) for inheritance
    ///     **2. Parent Comment Validation (Optional):**
    ///     - If ParentCommentId provided, validates parent comment exists
    ///     - Enables nested comment threads (replies)
    ///     - Throws CommentNotFoundException if parent not found
    ///     **3. Comment Entity Creation:**
    ///     - Creates Comment with trimmed content
    ///     - Associates with author profile
    ///     - Links to target (Post/JobPosting)
    ///     - Links to parent comment (if reply)
    ///     **4. Media Attachment Processing:**
    ///     If media provided:
    ///     - Iterates MediaObjectRequest collection
    ///     - Creates CommentMedia entities
    ///     - Processes via MediaAttachmentService
    ///     - S3 grant activation for upload
    ///     - Sanitized filenames
    ///     **5. Tag Inheritance:**
    ///     - Extracts tags from parent Post/JobPosting
    ///     - Adds tags to commenter's UserInterests
    ///     - Publishes UserInterestsUpdatedMessage
    ///     - Updates commenter's embedding for recommendations
    ///     **6. Content Moderation:**
    ///     - Publishes DetectInappropriateCommentMessage
    ///     - Background worker analyzes with AI
    ///     - Flags inappropriate content
    ///     - Sends moderation notifications
    ///     **7. Notifications:**
    ///     Sends real-time notifications to:
    ///     - Post author (new comment on their post)
    ///     - Parent comment author (reply to their comment)
    ///     - Via SignalR for instant delivery
    ///     **8. Response Projection:**
    ///     - Re-queries comment with full navigation properties
    ///     - Projects to CommentResponse DTO
    ///     - Includes author, media, vote counts
    ///     **Example Usage:**
    ///     <code>
    /// // Top-level comment on post
    /// var request = new CreateCommentRequest {
    ///     PostId = "post123",
    ///     Content = "Great article on Azure!",
    ///     Media = new List&lt;MediaObjectRequest&gt; {
    ///         new() { FileName = "screenshot.png", ContentType = "image/png" }
    ///     }
    /// };
    /// var comment = await commentService.CreateCommentAsync(request, currentProfile);
    ///
    /// // Nested reply to existing comment
    /// var replyRequest = new CreateCommentRequest {
    ///     PostId = "post123",
    ///     ParentCommentId = "comment456",
    ///     Content = "I agree with your point about scaling."
    /// };
    /// var reply = await commentService.CreateCommentAsync(replyRequest, currentProfile);
    /// </code>
    ///     **Tag Inheritance Example:**
    ///     <code>
    /// Post has tags: ["csharp", "azure", "devops"]
    ///     ↓
    /// User comments on post
    ///     ↓
    /// User's UserInterests updated: [...existing..., "csharp", "azure", "devops"]
    ///     ↓
    /// User's embedding regenerated
    ///     ↓
    /// More C#/Azure/DevOps content recommended to user
    /// </code>
    ///     **Media Upload Flow:**
    ///     <code>
    /// Client requests comment creation
    ///     ↓
    /// CommentMedia records created
    ///     ↓
    /// S3 grants activated (15-day expiration)
    ///     ↓
    /// Client uploads to presigned S3 URLs
    ///     ↓
    /// Media accessible via FileName + ContentType
    /// </code>
    ///     **Notification Delivery:**
    ///     - Instant: SignalR hub broadcasts to connected clients
    ///     - Persistent: Database Notification records for later retrieval
    ///     - Push: Mobile/web push notifications (future)
    ///     **Performance:**
    ///     - Single database transaction for all entities
    ///     - Bulk media processing
    ///     - Async notification delivery
    ///     - Background content moderation
    ///     **Error Handling:**
    ///     - Invalid PostId/JobPostingId: PostNotFoundException
    ///     - Invalid ParentCommentId: CommentNotFoundException
    ///     - Null request: ArgumentNullException
    ///     - Empty content: ArgumentException
    ///     This is the primary entry point for all comment creation in the platform.
    /// </remarks>
    public async Task<CommentResponse> CreateCommentAsync(
        CreateCommentRequest request,
        Profile authorProfile)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(authorProfile);

        // Validate request using FluentValidation
        var validationResult = await _createCommentValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var post = await _dbContext.Posts
            .Include(p => p.PostTags)
            .ThenInclude(pt => pt.Tag) // Include tags for tagging service
            .FirstOrDefaultAsync(p => p.Id == request.PostId);

        var jobPosting = await _dbContext.JobPostings
            .Include(p => p.JobPostingTags)
            .ThenInclude(pt => pt.Tag)
            .FirstOrDefaultAsync(p => p.Id == request.JobPostingId);

        if (post == null && jobPosting == null)
        {
            // Consider throwing a more specific BadHttpRequestException or similar if PostId is invalid
            throw new PostNotFoundException(
                $"Post/JobPosting with id=({request.PostId})/({request.JobPostingId}) was not found for comment creation.");
        }

        Comment? parentComment = null;
        if (!string.IsNullOrEmpty(request.ParentCommentId))
        {
            parentComment = await _dbContext.Comments
                .FirstOrDefaultAsync(c => c.Id == request.ParentCommentId);
            if (parentComment == null)
            {
                throw new CommentNotFoundException($"Parent comment with id={request.ParentCommentId} was not found.");
            }
        }

        var comment = new Comment
        {
            AuthorId = authorProfile.Id,
            Author = authorProfile,
            Content = request.Content.Trim(),
            ParentCommentId = request.ParentCommentId, // Can be null
            ParentComment = parentComment, // Can be null
            Post = post,
            PostId = request.PostId,
            JobPosting = jobPosting,
            JobPostingId = request.JobPostingId
        };

        await _dbContext.Comments.AddAsync(comment);

        if (request.Media?.Count > 0)
        {
            CommentMedia createCommentMediaFunc(MediaObjectRequest mediaReq, Comment c)
            {
                return new CommentMedia
                {
                    Comment = c,
                    Name = _mediaService.SanitizeMediaName(c.Content), // Use a helper for safety
                    Type = mediaReq.Type,
                    Key = mediaReq.Key
                };
            }

            comment.Medias = await _mediaService.ProcessAndAttachMediaAsync(
                request.Media,
                comment,
                createCommentMediaFunc,
                _dbContext
            );
        }

        await _dbContext.SaveChangesAsync(); // Single save point for comment, media, grants, notifications

        if (post != null)
        {
            await _taggingService.AddTagsToUserProfileAsync(
                authorProfile.Id,
                post.PostTags.Select(pt => pt.Tag)
            );
        }
        else if (jobPosting != null)
        {
            await _taggingService.AddTagsToUserProfileAsync(
                authorProfile.Id,
                jobPosting.JobPostingTags.Select(pt => pt.Tag)
            );
        }

        // Post-save actions
        await _publishEndpoint.Publish(new DetectInappropriateCommentMessage
        {
            CommentId = comment.Id, // Id is now populated
            Content = comment.Content,
            AuthorId = comment.AuthorId
        });

        // Actual dispatch of notifications
        await _notificationFacade.NotifyNewCommentAsync(comment);

        // Re-fetch to ensure all navigations are loaded for the response,
        // especially if Author, Medias, Votes etc. are needed by SelectCommentResponseFromFullComment
        // and weren't explicitly loaded or fixed up on the 'comment' instance.
        // Using FirstOrDefaultAsync as Id is unique.
        //var createdCommentWithIncludes = await _dbContext.Comments
        //    .FullyPopulatedCommentQuery(c => c.Id == comment.Id)
        //    .FirstOrDefaultAsync();

        //if (createdCommentWithIncludes == null)
        //{
        //    // This should not happen if SaveChangesAsync() was successful and Id is correct.
        //    // Log error.
        //    throw new InvalidOperationException(
        //        "Failed to retrieve the comment after creation, indicating a data consistency issue.");
        //}

        return comment.SelectCommentResponseFromFullComment(authorProfile.Id);
    }

    /// <summary>
    ///     Retrieves a single comment by ID with full metadata including author, media, and vote counts.
    /// </summary>
    /// <param name="commentId">The unique identifier of the comment to retrieve.</param>
    /// <param name="userProfileId">
    ///     Optional profile ID of the requesting user to determine their vote status on this comment.
    ///     If null, vote status will be null in the response.
    /// </param>
    /// <returns>
    ///     A task containing the comment response with all metadata, or null if the comment doesn't exist.
    /// </returns>
    /// <remarks>
    ///     **Query Optimizations:**
    ///     - Uses FullyPopulatedCommentQuery extension for eager loading
    ///     - Loads: Author, CommentMedia, CommentVotes
    ///     - AsNoTracking for read-only performance
    ///     - Projects to CommentResponse DTO
    ///     **Response Data:**
    ///     - Comment content and metadata
    ///     - Author profile (name, avatar)
    ///     - Attached media (URLs, types)
    ///     - Vote counts (upvotes, downvotes)
    ///     - Current user's vote status (if userProfileId provided)
    ///     **Example Usage:**
    ///     <code>
    /// // Anonymous user viewing comment
    /// var comment = await commentService.GetCommentAsync("comment123", null);
    ///
    /// // Authenticated user viewing comment (includes their vote status)
    /// var comment = await commentService.GetCommentAsync("comment123", currentUser.ProfileId);
    /// if (comment.CurrentUserVote == VoteType.Upvote) {
    ///     // User has upvoted this comment
    /// }
    /// </code>
    ///     Returns null if comment not found (no exception thrown).
    /// </remarks>
    public async Task<CommentResponse?> GetCommentAsync(string commentId, string? userProfileId)
    {
        ArgumentException.ThrowIfNullOrEmpty(commentId, nameof(commentId));

        var comment = await _dbContext.Comments
            .FullyPopulatedCommentQuery(c => c.Id == commentId) // Uses your existing query extension
            .SelectCommentResponseFromFullComment(userProfileId)
            .FirstOrDefaultAsync();

        return comment;
    }

    /// <summary>
    ///     Retrieves all comments for a specific post, ordered chronologically.
    /// </summary>
    /// <param name="postId">The unique identifier of the post whose comments to retrieve.</param>
    /// <param name="userProfileId">
    ///     Optional profile ID of the requesting user to determine their vote status on each comment.
    /// </param>
    /// <returns>A task containing a list of comment responses with full metadata.</returns>
    /// <exception cref="PostNotFoundException">Thrown when the post doesn't exist.</exception>
    /// <remarks>
    ///     **Post Validation:**
    ///     Throws PostNotFoundException if postId invalid, preventing queries on non-existent posts.
    ///     **Query Characteristics:**
    ///     - Filters by PostId
    ///     - Eager loads: Author, CommentMedia, CommentVotes
    ///     - Orders by CreatedAt ascending (oldest first)
    ///     - Projects to CommentResponse DTOs
    ///     - AsNoTracking for performance
    ///     **Comment Threading:**
    ///     Returns flat list including both top-level and nested comments.
    ///     Client responsible for building tree structure using ParentCommentId.
    ///     **Example Usage:**
    ///     <code>
    /// var comments = await commentService.GetCommentsByPostAsync("post123", currentUser.ProfileId);
    ///
    /// // Build comment tree client-side
    /// var topLevel = comments.Where(c => c.ParentCommentId == null);
    /// foreach (var comment in topLevel) {
    ///     var replies = comments.Where(c => c.ParentCommentId == comment.Id);
    ///     // Render comment with nested replies
    /// }
    /// </code>
    ///     Useful for displaying all discussion on a post, including nested reply threads.
    /// </remarks>
    public async Task<List<CommentResponse>> GetCommentsByPostAsync(string postId, string? userProfileId)
    {
        ArgumentException.ThrowIfNullOrEmpty(postId, nameof(postId));

        // Check if post exists - this is good practice to ensure valid foreign key
        var postExists = await _dbContext.Posts.AnyAsync(p => p.Id == postId);
        if (!postExists)
        {
            throw new PostNotFoundException($"Post with id={postId} was not found for retrieving comments.");
        }

        var commentEntities = await _dbContext.Comments
            .FullyPopulatedCommentQuery(c => c.PostId == postId)
            .SelectCommentResponseFromFullComment(userProfileId)
            .ToListAsync();

        return commentEntities;
    }

    /// <summary>
    ///     Retrieves all comments for a specific job posting, ordered chronologically.
    /// </summary>
    /// <param name="jobPostingId">The unique identifier of the job posting whose comments to retrieve.</param>
    /// <param name="userProfileId">
    ///     Optional profile ID of the requesting user to determine their vote status on each comment.
    /// </param>
    /// <returns>A task containing a list of comment responses with full metadata.</returns>
    /// <exception cref="JobPostingNotFoundException">Thrown when the job posting doesn't exist.</exception>
    /// <remarks>
    ///     **Job Posting Validation:**
    ///     Throws JobPostingNotFoundException if jobPostingId invalid.
    ///     **Use Cases:**
    ///     - Job posting Q&A section
    ///     - Clarification questions from contractors
    ///     - Client responses to questions
    ///     - Pre-application discussions
    ///     **Query Characteristics:**
    ///     - Filters by JobPostingId
    ///     - Eager loads: Author, CommentMedia, CommentVotes
    ///     - Orders by CreatedAt ascending
    ///     - Projects to CommentResponse DTOs
    ///     **Example:**
    ///     <code>
    /// // Display Q&A section on job posting page
    /// var comments = await commentService.GetCommentsByJobAsync(jobId, currentUser.ProfileId);
    /// foreach (var comment in comments) {
    ///     // Render question/answer
    /// }
    /// </code>
    ///     Identical behavior to GetCommentsByPostAsync but targets job postings.
    /// </remarks>
    public async Task<List<CommentResponse>> GetCommentsByJobAsync(string jobPostingId, string? userProfileId)
    {
        ArgumentException.ThrowIfNullOrEmpty(jobPostingId, nameof(jobPostingId));

        // Check if post exists - this is good practice to ensure valid foreign key
        var jobExists = await _dbContext.JobPostings.AnyAsync(p => p.Id == jobPostingId);
        if (!jobExists)
        {
            throw new PostNotFoundException(
                $"JobPosting with id={jobPostingId} was not found for retrieving comments.");
        }

        var commentEntities = await _dbContext.Comments
            .FullyPopulatedCommentQuery(c => c.JobPostingId == jobPostingId)
            .SelectCommentResponseFromFullComment(userProfileId)
            .ToListAsync();

        return commentEntities;
    }

    /// <summary>
    ///     Retrieves all comments authored by a specific user profile across all posts and job postings.
    /// </summary>
    /// <param name="profileId">The unique identifier of the profile whose comments to retrieve.</param>
    /// <param name="userProfileId">
    ///     Optional profile ID of the requesting user to determine their vote status on each comment.
    /// </param>
    /// <returns>A task containing a list of comment responses with full metadata.</returns>
    /// <exception cref="ProfileNotFoundException">Thrown when the profile doesn't exist.</exception>
    /// <remarks>
    ///     **Profile Validation:**
    ///     Throws ProfileNotFoundException if profileId invalid.
    ///     **Use Cases:**
    ///     - User profile page showing comment history
    ///     - Activity feed
    ///     - Reputation tracking
    ///     - Content moderation review
    ///     - User engagement analytics
    ///     **Query Characteristics:**
    ///     - Filters by AuthorId
    ///     - Eager loads: Author, CommentMedia, CommentVotes
    ///     - Orders by CreatedAt descending (most recent first)
    ///     - Projects to CommentResponse DTOs
    ///     - Includes comments on both posts AND job postings
    ///     **Example Usage:**
    ///     <code>
    /// // Display user's comment history on profile page
    /// var userComments = await commentService.GetCommentsByProfileAsync(
    ///     profileId,
    ///     currentUser.ProfileId
    /// );
    ///
    /// // Show statistics
    /// var totalComments = userComments.Count;
    /// var totalUpvotes = userComments.Sum(c => c.UpvotesCount);
    /// var avgScore = totalUpvotes / (double)totalComments;
    /// </code>
    ///     **Privacy:**
    ///     Returns all comments (public data).
    ///     Future enhancement: filter deleted/hidden comments for non-moderators.
    ///     Useful for tracking user engagement and building reputation scores.
    /// </remarks>
    public async Task<List<CommentResponse>> GetCommentsByProfileAsync(string profileId, string? userProfileId)
    {
        ArgumentException.ThrowIfNullOrEmpty(profileId, nameof(profileId));

        // Check if profile exists
        var profileExists = await _dbContext.Profiles.AnyAsync(p => p.Id == profileId);
        if (!profileExists)
        {
            throw new UserNotFoundException($"Profile with id={profileId} was not found for retrieving comments.");
        }

        // currentMaybeUserProfileId is passed for consistency, but for "comments by profile X",
        // the perspective for IsUpvoted/IsDownvoted is often the *requesting user*, not profileId.
        // If profileId is meant to be the perspective, then pass profileId to SelectCommentResponseFromFullComment.
        // Usually, it's the *current authenticated user's* perspective.
        var comments = await _dbContext.Comments
            .FullyPopulatedCommentQuery(c => c.AuthorId == profileId)
            .SelectCommentResponseFromFullComment(userProfileId)
            .ToListAsync();

        return comments;
    }

    /// <summary>
    ///     Edits an existing comment's content with authorization checks and content moderation.
    /// </summary>
    /// <param name="commentId">The unique identifier of the comment to edit.</param>
    /// <param name="request">The edit request containing the new content.</param>
    /// <param name="editorProfile">The authenticated user profile attempting the edit.</param>
    /// <returns>
    ///     A task containing the updated comment response, or null if comment not found after edit.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when request or editorProfile is null.</exception>
    /// <exception cref="ArgumentException">Thrown when commentId is null or empty.</exception>
    /// <exception cref="CommentNotFoundException">Thrown when the comment doesn't exist.</exception>
    /// <exception cref="UnauthorizedException">Thrown when the editor is not the comment author.</exception>
    /// <remarks>
    ///     **Authorization Model:**
    ///     Only the comment author can edit their own comments.
    ///     Moderators/admins cannot edit user comments (design decision for authenticity).
    ///     **Edit Workflow:**
    ///     1. Fetch comment (basic query without full eager loading for performance)
    ///     2. Verify comment exists → CommentNotFoundException
    ///     3. Verify editor is author → UnauthorizedException
    ///     4. Compare new content with existing (trimmed)
    ///     5. If changed:
    ///     - Update Content property
    ///     - SaveChangesAsync
    ///     - Publish DetectInappropriateCommentMessage for re-moderation
    ///     6. Re-fetch with full navigation properties
    ///     7. Project to CommentResponse
    ///     **Content Moderation:**
    ///     After edit, comment re-analyzed for inappropriate content:
    ///     <code>
    /// DetectInappropriateCommentMessage {
    ///     CommentId: "comment123",
    ///     Content: "Updated comment text",
    ///     AuthorId: "profile456"
    /// }
    /// </code>
    ///     Background worker checks for:
    ///     - Toxicity/harassment
    ///     - Spam
    ///     - Inappropriate language
    ///     - Policy violations
    ///     **No-Op Optimization:**
    ///     If content unchanged (after trimming), skips database write and moderation.
    ///     **Example Usage:**
    ///     <code>
    /// var editRequest = new EditCommentRequest {
    ///     Content = "Updated comment with additional context."
    /// };
    ///
    /// try {
    ///     var updated = await commentService.EditCommentAsync(
    ///         "comment123",
    ///         editRequest,
    ///         currentUserProfile
    ///     );
    /// } catch (UnauthorizedException) {
    ///     // User tried to edit someone else's comment
    /// }
    /// </code>
    ///     **Security Logging:**
    ///     Unauthorized edit attempts logged with warning level:
    ///     - Who attempted (editorProfile.Id)
    ///     - What comment (commentId)
    ///     - Actual owner (comment.AuthorId)
    ///     **Response Data:**
    ///     Re-fetches comment with FullyPopulatedCommentQuery to ensure:
    ///     - Author details
    ///     - Media attachments
    ///     - Vote counts (unchanged by edit)
    ///     - All navigation properties populated
    ///     Media attachments cannot be edited (limitation: delete comment and recreate if media changes needed).
    /// </remarks>
    public async Task<CommentResponse?> EditCommentAsync(string commentId, EditCommentRequest request,
        Profile editorProfile)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        ArgumentException.ThrowIfNullOrEmpty(commentId, nameof(commentId));
        ArgumentNullException.ThrowIfNull(editorProfile, nameof(editorProfile));

        // Validate request using FluentValidation
        var validationResult = await _editCommentValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var comment = await _dbContext.Comments
            // Include author to ensure we can map to CommentResponse, or FullyPopulatedCommentQuery does it.
            // .Include(c => c.Author)
            // .Include(c => c.Votes)
            // .Include(c => c.Medias)
            // For edit, we might only need the basic comment if not re-fetching with FullyPopulated later.
            // Let's fetch with enough data or plan to re-fetch.
            .FirstOrDefaultAsync(c => c.Id == commentId);

        if (comment == null)
        {
            throw new CommentNotFoundException(
                $"Comment with id={commentId} was not found for editing.");
        }

        if (comment.AuthorId != editorProfile.Id)
        {
            // Log this attempt
            _logger.LogWarning("User {EditorProfileId} attempted to edit comment {CommentId} owned by {AuthorId}.",
                editorProfile.Id, comment.Id, comment.AuthorId);
            throw new UnauthorizedException(
                $"User {editorProfile.Id} is not authorized to edit comment {commentId}.");
        }

        if (!string.IsNullOrWhiteSpace(request.Content) && comment.Content != request.Content.Trim())
        {
            comment.Content = request.Content.Trim();

            await _dbContext.SaveChangesAsync(); // Save only if actual changes were made

            // Offload to inappropriate content detection
            await _publishEndpoint.Publish(new DetectInappropriateCommentMessage
            {
                CommentId = comment.Id,
                Content = comment.Content,
                AuthorId = comment.AuthorId
            });
        }

        // Return the potentially updated comment, mapped to response
        // Re-fetch with full population to ensure response is complete.
        var updatedCommentEntity = await _dbContext.Comments
            .FullyPopulatedCommentQuery(c => c.Id == commentId)
            .SelectCommentResponseFromFullComment(editorProfile.Id)
            .FirstOrDefaultAsync();

        return updatedCommentEntity;
    }

    /// <summary>
    ///     Processes an upvote or downvote on a comment with toggle mechanics and notification delivery.
    /// </summary>
    /// <param name="commentId">The unique identifier of the comment to vote on.</param>
    /// <param name="voterProfile">The authenticated user profile casting the vote.</param>
    /// <param name="isUpvoteIntent">True for upvote, false for downvote.</param>
    /// <returns>A task containing the updated comment response with new vote counts.</returns>
    /// <exception cref="CommentNotFoundException">Thrown when the comment doesn't exist.</exception>
    /// <exception cref="VoteException">Thrown when user tries to vote on their own comment.</exception>
    /// <remarks>
    ///     **Toggle Voting Mechanics:**
    ///     **Scenario 1: No Existing Vote**
    ///     - User upvotes → Create CommentVote(IsUpvote=true)
    ///     - User downvotes → Create CommentVote(IsUpvote=false)
    ///     **Scenario 2: Clicking Same Vote**
    ///     - User upvoted, clicks upvote again → Delete CommentVote (vote removed)
    ///     - User downvoted, clicks downvote again → Delete CommentVote (vote removed)
    ///     **Scenario 3: Switching Vote Type**
    ///     - User upvoted, clicks downvote → Update CommentVote(IsUpvote=false)
    ///     - User downvoted, clicks upvote → Update CommentVote(IsUpvote=true)
    ///     **Self-Vote Prevention:**
    ///     Users cannot vote on their own comments.
    ///     Throws VoteException with message "You cannot vote on your own comment."
    ///     **Vote Processing Logic:**
    ///     <code>
    /// existingVote = Get current user's vote on this comment
    ///
    /// if (existingVote == null) {
    ///     // No vote yet, create new
    ///     CreateCommentVote(commentId, voterId, isUpvoteIntent)
    /// } else if (existingVote.IsUpvote == isUpvoteIntent) {
    ///     // Clicking same vote type, remove vote
    ///     DeleteCommentVote(existingVote)
    /// } else {
    ///     // Switching vote type, update
    ///     existingVote.IsUpvote = isUpvoteIntent
    /// }
    /// </code>
    ///     **Notification Delivery:**
    ///     When upvote added (not on removal or downvote):
    ///     - Sends notification to comment author
    ///     - Real-time delivery via SignalR
    ///     - Persistent notification record in database
    ///     - Message: "[User] upvoted your comment"
    ///     **Example Usage:**
    ///     <code>
    /// // User clicks upvote button
    /// var updated = await commentService.VoteCommentAsync(
    ///     "comment123",
    ///     currentUserProfile,
    ///     isUpvoteIntent: true
    /// );
    /// // updated.UpvotesCount incremented
    /// // updated.CurrentUserVote == VoteType.Upvote
    ///
    /// // User clicks upvote again (toggle off)
    /// updated = await commentService.VoteCommentAsync(
    ///     "comment123",
    ///     currentUserProfile,
    ///     isUpvoteIntent: true
    /// );
    /// // updated.UpvotesCount decremented
    /// // updated.CurrentUserVote == null
    /// </code>
    ///     **Vote Count Calculation:**
    ///     - UpvotesCount: Count of CommentVotes where IsUpvote=true
    ///     - DownvotesCount: Count of CommentVotes where IsUpvote=false
    ///     - Net score: UpvotesCount - DownvotesCount
    ///     **Performance:**
    ///     - Single database query to check existing vote
    ///     - Conditional insert/update/delete (minimal writes)
    ///     - Re-fetch with full navigation for accurate counts
    ///     - Async notification delivery (non-blocking)
    ///     **UI Integration:**
    ///     Response includes CurrentUserVote field:
    ///     - null: No vote cast
    ///     - VoteType.Upvote: User upvoted
    ///     - VoteType.Downvote: User downvoted
    ///     Client uses this to render button states (highlighted/unhighlighted).
    ///     **Future Enhancements:**
    ///     - Reputation points for comment author based on votes
    ///     - Vote weight based on voter reputation
    ///     - Downvote notifications (currently only upvotes notify)
    ///     - Vote fraud detection (rapid vote changes)
    ///     This method is critical for community engagement and content quality signals.
    /// </remarks>
    public async Task<CommentResponse> VoteCommentAsync(string commentId, Profile voterProfile, bool isUpvoteIntent)
    {
        ArgumentException.ThrowIfNullOrEmpty(commentId);
        ArgumentNullException.ThrowIfNull(voterProfile);

        // Fetch comment with necessary includes for vote processing and notification
        var comment = await _dbContext.Comments
            .Include(c => c.Author) // Needed for notification recipient
                                    // .Include(c => c.Votes) // Not strictly needed here if we query CommentVotes separately
            .FirstOrDefaultAsync(c => c.Id == commentId);

        if (comment == null)
        {
            throw new CommentNotFoundException($"Comment with id={commentId} was not found for voting.");
        }

        var vote = await _dbContext.CommentVotes
            // .Include(v => v.Comment) // Not needed if comment is already fetched
            // .ThenInclude(c => c.Author)
            .FirstOrDefaultAsync(v => v.CommentId == commentId && v.ProfileId == voterProfile.Id);

        if (vote == null)
        {
            vote = new CommentVote
            {
                ProfileId = voterProfile.Id,
                Profile = voterProfile,
                CommentId = comment.Id,
                Comment = comment,
                IsUpvote = isUpvoteIntent
            };

            await _dbContext.CommentVotes.AddAsync(vote);
        }
        else
        {
            if (vote.IsUpvote == isUpvoteIntent) // upvoting an already upvoted comment
            {
                _dbContext.CommentVotes.Remove(vote);
                vote = null;
            }
            else // upvoting a downvoted comment, or downvoting an upvoted one
            {
                vote.IsUpvote = isUpvoteIntent;
            }
        }

        await _dbContext.SaveChangesAsync();

        if (vote != null)
        {
            // Ensure voteToNotify.Comment and voteToNotify.Profile (voterProfile) are loaded for the facade
            // voteToNotify.Comment is 'comment' which has Author loaded.
            // voteToNotify.Profile will be voterProfile.

            vote.Comment = comment;
            await _notificationFacade.NotifyCommentVotedAsync(vote);
        }

        // Re-fetch the comment with all details for the response
        var updatedComment = await _dbContext.Comments
            .FullyPopulatedCommentQuery(c => c.Id == commentId)
            .SelectCommentResponseFromFullComment(voterProfile.Id)
            .FirstAsync();

        return updatedComment;
    }

    // <summary>
    ///     Deletes a comment with authorization checks and cascade handling for nested replies.
    /// </summary>
    /// <param name="commentId">The unique identifier of the comment to delete.</param>
    /// <param name="deleterProfile">The authenticated user profile attempting the deletion.</param>
    /// <returns>
    ///     A task containing true if deletion successful or comment already deleted (idempotent),
    ///     false if deletion failed.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when commentId is null or empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when deleterProfile is null.</exception>
    /// <exception cref="ForbiddenAccessException">Thrown when the deleter is not the comment author.</exception>
    /// <remarks>
    ///     **Authorization Model:**
    ///     Only the comment author can delete their own comments.
    ///     Future enhancement: Allow moderators/admins to delete any comment.
    ///     **Idempotent Deletion:**
    ///     If comment not found, returns true (HTTP 204 semantics: resource doesn't exist = success state).
    ///     This prevents errors from duplicate DELETE requests.
    ///     **Cascade Deletion:**
    ///     When a comment is deleted, the following are also removed via EF Core cascade delete:
    ///     - All CommentVotes on this comment
    ///     - All CommentMedia attachments
    ///     - All nested replies (Comments where ParentCommentId == this comment's ID)
    ///     **WARNING:** Deleting a comment with replies removes entire thread:
    ///     <code>
    /// Comment A
    ///     ↓ ParentCommentId
    /// Comment B (reply to A)
    ///     ↓ ParentCommentId
    /// Comment C (reply to B)
    ///
    /// DELETE Comment A → Cascade deletes B and C
    /// </code>
    ///     **Cascade Configuration:**
    ///     Defined in CommentEntityConfiguration:
    ///     <code>
    /// builder.HasMany(c => c.Replies)
    ///     .WithOne(c => c.ParentComment)
    ///     .OnDelete(DeleteBehavior.Cascade);
    /// </code>
    ///     **Alternative Approach (Not Implemented):**
    ///     Soft delete pattern:
    ///     - Set IsDeleted = true
    ///     - Set Content = "[deleted]"
    ///     - Keep comment visible with placeholder
    ///     - Preserve nested reply threads
    ///     **Example Usage:**
    ///     <code>
    /// try {
    ///     var deleted = await commentService.DeleteCommentAsync(
    ///         "comment123",
    ///         currentUserProfile
    ///     );
    ///     if (deleted) {
    ///         // Comment and all replies removed
    ///     }
    /// } catch (ForbiddenAccessException) {
    ///     // User tried to delete someone else's comment
    /// }
    /// </code>
    ///     **Security Considerations:**
    ///     - Authorization checked before deletion
    ///     - Commented code shows pattern for moderator override
    ///     - Failed attempts should be logged (currently commented)
    ///     **Database Effects:**
    ///     Single transaction removes:
    ///     1. Comment entity
    ///     2. CommentVotes (cascade)
    ///     3. CommentMedia (cascade)
    ///     4. Nested replies (cascade)
    ///     5. PostTag/JobPostingTag associations (if any)
    ///     **UI Considerations:**
    ///     Should warn user before deleting comments with replies:
    ///     "This will delete X replies. Are you sure?"
    ///     **Future Enhancements:**
    ///     - Soft delete for better user experience
    ///     - Moderator deletion with audit log
    ///     - Restore deleted comments within time window
    ///     - Archive instead of hard delete
    ///     This method provides permanent deletion with cascade for data integrity.
    /// </remarks>
    public async Task<bool> DeleteCommentAsync(string commentId, Profile deleterProfile)
    {
        ArgumentException.ThrowIfNullOrEmpty(commentId, nameof(commentId));
        ArgumentNullException.ThrowIfNull(deleterProfile, nameof(deleterProfile));

        var comment = await _dbContext.Comments
            .Include(c => c.Replies)
            .FirstOrDefaultAsync(c => c.Id == commentId);

        if (comment == null)
        {
            // Don't throw if not found for DELETE, just return false (idempotency)
            // Or, if you prefer to throw, the controller should catch and return NoContent still.
            // For strict idempotency, "resource not found" is a success state for DELETE.
            return true; // Or false if you want to signal "wasn't there to delete"
        }

        if (comment.AuthorId != deleterProfile.Id)
        {
            //Optionally, check for admin / moderator roles if they can delete others' comments
            //if (!await _userService.IsAdminOrModeratorAsync(deleterProfile))
            //{
            //    _logger.LogWarning("User {DeleterProfileId} attempted to delete comment {CommentId} owned by {AuthorId}.", deleterProfile.Id, comment.Id, comment.AuthorId);
            //    throw new ForbiddenAccessException($"User {deleterProfile.Id} is not authorized to delete comment {commentId}.");
            //}

            throw new ForbiddenAccessException(
                $"User {deleterProfile.Id} is not authorized to delete comment {commentId}.");
        }

        _dbContext.Comments.Remove(comment);
        await _dbContext.SaveChangesAsync();

        return true;
    }
}
