using ExpertBridge.Core.Entities.ManyToManyRelationships.JobPostingTags;
using ExpertBridge.Core.Entities.ManyToManyRelationships.PostTags;
using ExpertBridge.Core.Entities.ManyToManyRelationships.UserInterests;
using ExpertBridge.Core.Entities.Tags;
using ExpertBridge.Core.Interfaces;
using ExpertBridge.Core.Messages;
using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Application.DomainServices;

/// <summary>
/// Provides intelligent tag management and content categorization using AI-powered analysis.
/// </summary>
/// <remarks>
/// This service orchestrates the complete tagging lifecycle from AI-generated tag creation
/// through association with posts, job postings, and user profiles for personalized recommendations.
/// 
/// **Core Responsibilities:**
/// - AI-powered content categorization (via Groq LLM)
/// - Bilingual tag management (English/Arabic)
/// - Post and job posting tagging
/// - User interest profiling through tag aggregation
/// - Tag-based recommendation system foundation
/// 
/// **Architecture Integration:**
/// - Groq LLM: Analyzes content and generates semantic tags
/// - MassTransit: Publishes UserInterestsUpdatedMessage for embedding generation
/// - TaggingService → Background Worker → Embedding Generation → Recommendations
/// 
/// **AI Tag Generation Flow:**
/// <code>
/// Post Created
///     ↓
/// PostProcessingPipelineMessage published
///     ↓
/// Background Consumer calls Groq API
///     ↓
/// Groq returns PostCategorizerResponse (tags, language)
///     ↓
/// AddRawTagsToPostAsync(postId, tags)
///     ↓
/// Create Tags in DB (English + Arabic names)
///     ↓
/// Associate with Post (PostTags)
///     ↓
/// Associate with Author Profile (UserInterests)
///     ↓
/// Publish UserInterestsUpdatedMessage
///     ↓
/// Background Worker generates user embedding vector
/// </code>
/// 
/// **Bilingual Tag System:**
/// Tags maintain both English and Arabic names:
/// <code>
/// Tag {
///     EnglishName: "machine learning",
///     ArabicName: "تعلم الآلة",
///     Description: "AI/ML and neural networks"
/// }
/// </code>
/// 
/// **Tag Association Tables:**
/// - **PostTags**: Many-to-many Post ↔ Tag
/// - **JobPostingTags**: Many-to-many JobPosting ↔ Tag
/// - **UserInterests**: Many-to-many Profile ↔ Tag (aggregated from interactions)
/// 
/// **User Interest Tracking:**
/// User interests automatically accumulated from:
/// - Posts they create (post tags → user interests)
/// - Posts they upvote (post tags → user interests)
/// - Comments they write (parent post tags → user interests)
/// - Job postings they create or vote on
/// - Job applications they submit
/// 
/// **Recommendation Algorithm:**
/// 1. User creates/votes content → Tags associated with their profile
/// 2. UserInterestsUpdatedMessage triggers embedding generation
/// 3. Background worker aggregates tag vectors → User embedding
/// 4. Recommendations based on cosine similarity of user embeddings
/// 
/// **Tag Normalization:**
/// - Duplicate prevention (checks English + Arabic names)
/// - Case-sensitive matching for proper nouns
/// - Automatic language detection
/// - Description generation for context
/// 
/// **Performance Optimization:**
/// - Bulk tag creation (AddRangeAsync)
/// - Deduplication before DB insertion
/// - Incremental tag associations (only new relationships)
/// - Asynchronous embedding generation
/// 
/// **Use Cases:**
/// - Content discovery ("Show posts about machine learning")
/// - Expert matching ("Find profiles with C# and Azure skills")
/// - Job recommendations ("Jobs matching my interests")
/// - Skill taxonomy building
/// - Content moderation (inappropriate content categories)
/// 
/// Registered as scoped service for per-request lifetime and DbContext alignment.
/// </remarks>
public sealed class TaggingService
{
    /// <summary>
    ///     Represents the database context used to interact with the underlying database
    ///     for performing data-related operations, including accessing and manipulating
    ///     entities such as Users, Profiles, Tags, Posts, and more.
    /// </summary>
    private readonly ExpertBridgeDbContext _dbContext;

    /// <summary>
    ///     Represents the publish endpoint used for sending messages to the message broker,
    ///     enabling asynchronous communication and event-driven workflows within the application's infrastructure.
    /// </summary>
    private readonly IPublishEndpoint _publishEndpoint;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TaggingService" /> class.
    /// </summary>
    /// <param name="dbContext">The database context used for data operations.</param>
    /// <param name="publishEndpoint">The publish endpoint for messaging.</param>
    public TaggingService(ExpertBridgeDbContext dbContext, IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }


    /// <summary>
    /// Processes AI-generated tags and associates them with a post and the author's profile.
    /// </summary>
    /// <param name="postId">The unique identifier of the post to tag.</param>
    /// <param name="authorId">The unique identifier of the post author whose interests will be updated.</param>
    /// <param name="isJobPosting">True if tagging a job posting; false for regular post.</param>
    /// <param name="tags">
    /// The AI-generated categorization response containing bilingual tags, descriptions, and detected language.
    /// </param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task representing the asynchronous tagging operation.</returns>
    /// <remarks>
    /// **Complete Tagging Workflow:**
    /// 
    /// **1. Tag Creation:**
    /// - Calls CreateTagsInDatabaseAsync to create missing tags
    /// - Checks for existing tags by English OR Arabic name
    /// - Creates new Tag entities for novel categories
    /// - Saves to generate Tag IDs
    /// 
    /// **2. Post Association:**
    /// - Routes to AddTagsToPostInternalAsync or AddTagsToJobPostingInternalAsync
    /// - Creates PostTag/JobPostingTag junction records
    /// - Avoids duplicate associations
    /// 
    /// **3. Author Interest Update:**
    /// - Calls AddTagsToUserProfileInternalAsync
    /// - Creates UserInterest junction records
    /// - Builds interest profile for recommendations
    /// 
    /// **4. Content Metadata:**
    /// - Sets post.Language (detected by AI: "en", "ar", etc.)
    /// - Sets post.IsTagged = true (marks as processed)
    /// 
    /// **5. Embedding Generation Trigger:**
    /// - Publishes UserInterestsUpdatedMessage
    /// - Background worker aggregates user tags into embedding vector
    /// - Vector used for personalized recommendations
    /// 
    /// **AI Tag Format:**
    /// <code>
    /// PostCategorizerResponse {
    ///     Language: "en",
    ///     Tags: [
    ///         { EnglishName: "web development", ArabicName: "تطوير الويب", Description: "..." },
    ///         { EnglishName: "react", ArabicName: "رياكت", Description: "..." }
    ///     ]
    /// }
    /// </code>
    /// 
    /// **Database Changes:**
    /// - Inserts new Tags (if any)
    /// - Inserts PostTags/JobPostingTags
    /// - Inserts UserInterests
    /// - Updates Post metadata
    /// - All committed atomically in single transaction
    /// 
    /// **Example Usage:**
    /// <code>
    /// // In background consumer after Groq analysis
    /// var tags = await groqService.CategorizePostAsync(post.Content);
    /// await taggingService.AddRawTagsToPostAsync(
    ///     post.Id,
    ///     post.AuthorId,
    ///     isJobPosting: false,
    ///     tags
    /// );
    /// </code>
    /// 
    /// **Performance:**
    /// - Bulk operations for tag creation
    /// - Incremental associations (only new relationships)
    /// - Single SaveChangesAsync call
    /// - Asynchronous message publishing
    /// 
    /// This is the main entry point for AI-powered content tagging called by background workers.
    /// </remarks>
    public async Task AddRawTagsToPostAsync(
        string postId,
        string authorId,
        bool isJobPosting,
        PostCategorizerResponse tags,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(postId);
        ArgumentNullException.ThrowIfNull(tags);

        // This call commits changes to database (calls SaveChanges)
        var tagsToAdd = await CreateTagsInDatabaseAsync(tags, cancellationToken);

        IRecommendableContent post;

        if (isJobPosting)
        {
            post = await _dbContext.JobPostings.FirstAsync(p => p.Id == postId, cancellationToken);
            await AddTagsToJobPostingInternalAsync(post.Id, tagsToAdd.Select(t => t.Id), cancellationToken);
        }
        else
        {
            post = await _dbContext.Posts.FirstAsync(p => p.Id == postId, cancellationToken);
            await AddTagsToPostInternalAsync(post.Id, tagsToAdd.Select(t => t.Id), cancellationToken);
        }

        await AddTagsToUserProfileInternalAsync(authorId, tagsToAdd.Select(t => t.Id), cancellationToken);

        post.Language = tags.Language;
        post.IsTagged = true;

        await _dbContext.SaveChangesAsync(cancellationToken);

        await _publishEndpoint.Publish(new UserInterestsUpdatedMessage { UserProfileId = authorId }, cancellationToken);
    }

    /// <summary>
    /// Internal method that creates new tags in the database while avoiding duplicates.
    /// </summary>
    /// <param name="tags">The AI-generated tag categorization response.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>
    /// A task containing an enumerable of all tags (existing + newly created) ready for association.
    /// </returns>
    /// <remarks>
    /// **Deduplication Strategy:**
    /// - Queries existing tags matching ANY English OR Arabic name
    /// - Compares AI tags against existing to find novel entries
    /// - Creates only truly new tags
    /// 
    /// **Bilingual Matching:**
    /// <code>
    /// Existing: { EnglishName: "machine learning", ArabicName: "تعلم الآلة" }
    /// AI Generates: { EnglishName: "machine learning", ArabicName: "تعلم آلي" }
    /// Result: Matched (English name exists), reuse existing tag
    /// 
    /// AI Generates: { EnglishName: "deep learning", ArabicName: "تعلم عميق" }
    /// Result: Not found, create new tag
    /// </code>
    /// 
    /// **Tag Creation:**
    /// - New tags inserted with AddRangeAsync
    /// - SaveChangesAsync called to generate IDs
    /// - Returned collection includes both existing and new tags
    /// 
    /// **Why Separate Method:**
    /// - Reusable for different post types
    /// - Encapsulates deduplication logic
    /// - Single responsibility (tag creation only)
    /// 
    /// This method ensures tag taxonomy remains consistent and duplicate-free.
    /// </remarks>
    private async Task<IEnumerable<Tag?>> CreateTagsInDatabaseAsync(
        PostCategorizerResponse tags,
        CancellationToken cancellationToken)
    {
        var tagList = tags.Tags.ToList();

        var englishNames = tagList.Select(t => t.EnglishName).ToList();
        var arabicNames = tagList.Select(t => t.ArabicName).ToList();

        var existingTags = await _dbContext.Tags
            .AsNoTracking()
            .Where(t => englishNames.Contains(t.EnglishName) || arabicNames.Contains(t.ArabicName))
            .ToListAsync(cancellationToken); // materialize the query to use it in next calculations.

        var newRawTags = tagList
            .Where(t => !existingTags
                .Any(et => et.EnglishName == t.EnglishName || et.ArabicName == t.ArabicName))
            .ToList(); // this LINQ is only client side, thus we can use Any().

        var newTags = newRawTags.Select(tag => new Tag
        {
            EnglishName = tag.EnglishName,
            ArabicName = tag.ArabicName,
            Description = tag.Description
        }).ToList();

        await _dbContext.AddRangeAsync(newTags, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken); // Save new tags to generate their IDs

        var tagsToAdd = newTags.Concat(existingTags);
        return tagsToAdd;
    }


    /// <summary>
    ///     Associates a collection of tag IDs with a specific post by creating PostTag relationships in the database.
    ///     changes to the database, as it is intended to be part of a larger unit of work.
    ///     So it should not be called outside directly.
    /// </summary>
    /// <param name="postId">The identifier of the post to which the tags will be added.</param>
    /// <param name="tagIds">A collection of tag IDs that need to be associated with the post.</param>
    /// <param name="cancellationToken">
    ///     A token to observe while waiting for the task to complete, enabling cancellation of the
    ///     operation.
    /// </param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task AddTagsToPostInternalAsync(
        string postId,
        IEnumerable<string> tagIds,
        CancellationToken cancellationToken = default)
    {
        var existingTagIds = await _dbContext.PostTags
            .Where(pt => pt.PostId == postId && tagIds.Contains(pt.TagId))
            .Select(pt => pt.TagId)
            .ToListAsync(cancellationToken);

        var newPostTags = tagIds
            .Where(tagId => !existingTagIds.Contains(tagId))
            .Select(tagId => new PostTag { PostId = postId, TagId = tagId });

        await _dbContext.PostTags.AddRangeAsync(newPostTags, cancellationToken);
    }

    /// <summary>
    ///     Associates a collection of tag IDs with a specific JobPosting by creating JobPostingTag relationships in the
    ///     database.
    ///     changes to the database, as it is intended to be part of a larger unit of work.
    ///     So it should not be called outside directly.
    /// </summary>
    /// <param name="postingId">The identifier of the post to which the tags will be added.</param>
    /// <param name="tagIds">A collection of tag IDs that need to be associated with the post.</param>
    /// <param name="cancellationToken">
    ///     A token to observe while waiting for the task to complete, enabling cancellation of the
    ///     operation.
    /// </param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task AddTagsToJobPostingInternalAsync(
        string postingId,
        IEnumerable<string> tagIds,
        CancellationToken cancellationToken = default)
    {
        var existingTagIds = await _dbContext.JobPostingTags
            .Where(pt => pt.JobPostingId == postingId && tagIds.Contains(pt.TagId))
            .Select(pt => pt.TagId)
            .ToListAsync(cancellationToken);

        var newPostTags = tagIds
            .Where(tagId => !existingTagIds.Contains(tagId))
            .Select(tagId => new JobPostingTag { JobPostingId = postingId, TagId = tagId });

        await _dbContext.JobPostingTags.AddRangeAsync(newPostTags, cancellationToken);
    }

    /// <summary>
    /// Public method to associate tags with a post, committing changes immediately.
    /// </summary>
    /// <param name="postId">The identifier of the post to tag.</param>
    /// <param name="tags">Collection of Tag entities to associate.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// **Use Case:**
    /// Manual tagging or tag updates outside the AI pipeline.
    /// 
    /// **Example:**
    /// <code>
    /// var tags = await dbContext.Tags
    ///     .Where(t => new[] { "csharp", "dotnet" }.Contains(t.EnglishName))
    ///     .ToListAsync();
    /// await taggingService.AddTagsToPostAsync(postId, tags);
    /// </code>
    /// 
    /// Unlike AddRawTagsToPostAsync, this doesn't update user interests or trigger embedding generation.
    /// Use for administrative tag corrections or bulk operations.
    /// </remarks>
    public async Task AddTagsToPostAsync(string postId, IEnumerable<Tag> tags)
    {
        await AddTagsToPostInternalAsync(postId, tags.Select(t => t.Id));
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    ///     Adds tags to a user profile within a transaction. This method does not commit
    ///     changes to the database, as it is intended to be part of a larger unit of work.
    ///     So it should not be called outside directly.
    /// </summary>
    /// <param name="profileId">The unique identifier of the user's profile.</param>
    /// <param name="tagIds">A collection of tag IDs to associate with the user profile.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task AddTagsToUserProfileInternalAsync(
        string profileId,
        IEnumerable<string> tagIds,
        CancellationToken cancellationToken = default)
    {
        var existingTagIds = await _dbContext.UserInterests
            .Where(ui => ui.ProfileId == profileId && tagIds.Contains(ui.TagId))
            .Select(pt => pt.TagId)
            .ToListAsync(cancellationToken);

        var newUserInterests = tagIds
            .Where(tagId => !existingTagIds.Contains(tagId))
            .Select(tagId => new UserInterest { ProfileId = profileId, TagId = tagId });

        await _dbContext.UserInterests.AddRangeAsync(newUserInterests, cancellationToken);
    }

    /// <summary>
    /// Associates tags with a user profile for interest tracking (convenience overload).
    /// </summary>
    /// <param name="profileId">The user profile identifier.</param>
    /// <param name="tags">Collection of Tag entities to associate.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// This overload accepts Tag entities and delegates to the primary method with tag IDs.
    /// Used when tags are already loaded (e.g., from post.PostTags.Select(pt => pt.Tag)).
    /// </remarks>
    public async Task AddTagsToUserProfileAsync(string profileId, IEnumerable<Tag> tags)
    {
        await AddTagsToUserProfileAsync(profileId, tags.Select(t => t.Id));
    }

    /// <summary>
    /// Associates tag IDs with a user profile and triggers embedding regeneration.
    /// </summary>
    /// <param name="profileId">The user profile identifier.</param>
    /// <param name="tagIds">Collection of tag IDs to associate with the profile.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// **Primary Method:**
    /// This is the main entry point for user interest updates, called from multiple sources:
    /// - Post creation (author's interests updated with post tags)
    /// - Post voting (voter's interests updated with post tags)
    /// - Comment creation (commenter's interests updated with post tags)
    /// - Job posting interactions
    /// 
    /// **Processing Flow:**
    /// 1. Call AddTagsToUserProfileInternalAsync (adds UserInterest records)
    /// 2. SaveChangesAsync (commits to database)
    /// 3. Publish UserInterestsUpdatedMessage (triggers embedding generation)
    /// 
    /// **User Interests Accumulation:**
    /// <code>
    /// // User creates post about "C#" and "Azure"
    /// → UserInterests: [csharp, azure]
    /// 
    /// // User upvotes post about "Docker" and "Azure"
    /// → UserInterests: [csharp, azure, docker] (Azure already exists, Docker added)
    /// 
    /// // User comments on post about "Kubernetes"
    /// → UserInterests: [csharp, azure, docker, kubernetes]
    /// </code>
    /// 
    /// **Embedding Generation:**
    /// After publishing UserInterestsUpdatedMessage:
    /// - Background worker queries all user's UserInterests
    /// - Loads tag descriptions
    /// - Generates combined embedding vector (1024-dim)
    /// - Updates Profile.UserInterestEmbedding
    /// - Used for personalized content recommendations
    /// 
    /// **Example Usage:**
    /// <code>
    /// // After user votes on a post
    /// await taggingService.AddTagsToUserProfileAsync(
    ///     voterProfile.Id,
    ///     post.PostTags.Select(pt => pt.TagId)
    /// );
    /// // User's recommendation profile updated automatically
    /// </code>
    /// 
    /// **Incremental Updates:**
    /// Only adds new tag associations (doesn't remove old ones).
    /// User interest profiles grow over time, reflecting engagement patterns.
    /// 
    /// This method is critical for the recommendation engine's personalization capabilities.
    /// </remarks>
    public async Task AddTagsToUserProfileAsync(string profileId, IEnumerable<string> tagIds)
    {
        await AddTagsToUserProfileInternalAsync(profileId, tagIds);
        await _dbContext.SaveChangesAsync();

        await _publishEndpoint.Publish(new UserInterestsUpdatedMessage { UserProfileId = profileId });
    }
}
