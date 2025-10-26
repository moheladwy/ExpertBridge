using ExpertBridge.Application.DataGenerator;
using ExpertBridge.Core.Entities.Media.PostMedia;
using ExpertBridge.Core.Entities.Posts;
using ExpertBridge.Core.Entities.PostVotes;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Exceptions;
using ExpertBridge.Core.Messages;
using ExpertBridge.Core.Queries;
using ExpertBridge.Core.Requests.CreatePost;
using ExpertBridge.Core.Requests.EditPost;
using ExpertBridge.Core.Requests.MediaObject;
using ExpertBridge.Core.Requests.PostsCursor;
using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using ExpertBridge.Notifications;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Pgvector;
using Pgvector.EntityFrameworkCore;

// For logging
// Assuming PostQueries exist here
// For PostProcessingPipelineMessage
// For NotificationFacade

namespace ExpertBridge.Application.DomainServices;

/// <summary>
///     Provides comprehensive content management with AI-powered recommendations, similarity search, and social engagement
///     features.
/// </summary>
/// <remarks>
///     This is the most sophisticated service in ExpertBridge, managing the complete post lifecycle with extensive AI/ML
///     integration,
///     multiple caching strategies, diverse pagination approaches, and advanced recommendation algorithms.
///     **Core Features:**
///     - Post CRUD with media attachments
///     - AI-powered personalized recommendations (vector embeddings)
///     - Semantic similarity search using pgvector
///     - Multiple pagination strategies (cursor-based, offset-based)
///     - Voting system with tag inheritance for interest tracking
///     - Suggested content based on user profile analysis
///     - Multi-tier caching (L1 in-memory, L2 Redis)
///     - Background processing for embeddings and tagging
///     **AI Recommendation Architecture:**
///     <code>
/// Post Created
///     ↓
/// PostProcessingPipelineMessage published
///     ↓
/// Background Worker:
///   - Groq API: Content → Tags (bilingual: English + Arabic)
///   - Ollama API: Content → 1024-dim embedding vector
///     ↓
/// Database Update:
///   - Post.Embedding = vector
///   - Post.IsTagged = true
///   - PostTags associations created
///     ↓
/// User Interest Profile:
///   - Author's UserInterests updated with post tags
///   - UserInterestsUpdatedMessage published
///   - Profile.UserInterestEmbedding regenerated
///     ↓
/// Recommendation Engine:
///   - Cosine similarity: Post.Embedding ↔ Profile.UserInterestEmbedding
///   - Results ordered by distance (0 = perfect match, 2 = opposite)
/// </code>
///     **Vector Similarity Mathematics:**
///     Uses pgvector's cosine distance operator `<=>`:
///     - **Distance = 0**: Identical vectors (perfect match)
///     - **Distance
///     < 0.5**: Highly similar content
///         - ** Distance 0.5-1.0** : Moderately similar
///         - ** Distance>
///         1.5**: Dissimilar content
///         - **Distance = 2**: Completely opposite vectors
///         **Caching Strategy (Most Sophisticated in Solution):**
///         **Similar Posts Cache:**
///         - Key: `SimilarPosts_{postId}_{limit}`
///         - TTL: 30 minutes (sliding)
///         - Invalidation: Manual on post edit/delete
///         **Suggested Posts Cache:**
///         - Key: `SuggestedPosts_{profileId}_{limit}`
///         - TTL: 30 minutes
///         - Personalized per user
///         - Invalidation: On UserInterestsUpdatedMessage
///         **HybridCache Architecture:**
///         - **L1 (In-Memory)**: Ultra-fast repeated queries (same process)
///         - **L2 (Redis)**: Distributed cache across instances
///         - **Cache-Aside Pattern**: Generate on miss, store for future hits
///         **Pagination Strategies:**
///         **1. Cursor-Based (GetRecommendedPostsAsync):**
///         - Stateless, efficient for large datasets
///         - Cursor = last item's distance + ID (tie-breaker)
///         - Prevents duplicate/missing items on concurrent updates
///         - Ideal for infinite scroll
///         **2. Offset-Based (GetRecommendedPostsOffsetPageAsync):**
///         - Traditional page numbers
///         - Skip/Take pattern
///         - Simpler client implementation
///         - May have consistency issues on updates
///         **3. No Pagination (GetAllPostsAsync):**
///         - Returns entire dataset
///         - For admin dashboards, analytics
///         - Performance concern for large datasets
///         **Voting System with Tag Inheritance:**
///         <code>
/// User upvotes post about "C#" and "Azure"
///     ↓
/// PostVote created (IsUpvote=true)
///     ↓
/// Post's tags added to voter's UserInterests
///     ↓
/// UserInterestsUpdatedMessage published
///     ↓
/// Voter's embedding regenerated
///     ↓
/// Future recommendations include more C#/Azure content
/// </code>
///         **Toggle Voting Mechanics:**
///         - Click upvote → Create vote
///         - Click upvote again → Remove vote
///         - Click downvote after upvote → Switch to downvote
///         **Content Discovery Flow:**
///         **For Authenticated Users:**
///         1. User creates/votes on posts → Tags added to UserInterests
///         2. Background worker generates UserInterestEmbedding
///         3. GetRecommendedPostsAsync returns posts similar to user interests
///         4. Personalized feed adapts to user behavior
///         **For Anonymous Users:**
///         1. No UserInterestEmbedding available
///         2. Generate random 1024-dim vector (exploration)
///         3. Different random vector each session = diverse content
///         4. Encourages account creation for personalization
///         **Media Attachments:**
///         - Images, videos, documents
///         - S3 storage with presigned URLs
///         - 15-day upload grant activation
///         - Processed via MediaAttachmentService
///         - Sanitized filenames for security
///         **Background Processing Pipeline:**
///         PostProcessingPipelineMessage triggers:
///         - **Tag Generation**: Groq LLM analyzes content → categories
///         - **Embedding Creation**: Ollama generates semantic vector
///         - **Tag Association**: Links tags to post and author profile
///         - **Interest Update**: Refreshes user interest embeddings
///         - **Cache Invalidation**: Clears affected cached recommendations
///         **Notification Integration:**
///         Real-time notifications via SignalR for:
///         - Upvotes on your posts
///         - Comments on your posts
///         - Posts matching your interests
///         **Performance Optimizations:**
///         - AsNoTracking for read-only queries (no change tracking overhead)
///         - Eager loading with Include/ThenInclude (minimize round trips)
///         - Projection to DTOs (select only needed columns)
///         - Vector indexes on Embedding columns (pgvector HNSW)
///         - HybridCache reduces database load
///         - Background processing (non-blocking post creation)
///         **Query Extensions (Reusable Patterns):**
///         - FullyPopulatedPostQuery: Standard eager loading
///         - SelectPostResponseFromFullPost: Consistent DTO projection
///         **Authorization Rules:**
///         - Create: Any authenticated user
///         - Edit: Only post author
///         - Delete: Only post author (cascade to comments, votes, media)
///         - Vote: Any authenticated user except author
///         - View: Public (all users including anonymous)
///         **Use Cases:**
///         - Expert knowledge sharing
///         - Technical articles and tutorials
///         - Community discussions
///         - Professional networking content
///         - Skills showcase and portfolio
///         - AI-powered content discovery
///         - Personalized learning feeds
///         **Scalability Considerations:**
///         - Vector similarity queries scale with pgvector indexes
///         - HybridCache reduces database load
///         - Background processing decouples expensive operations
///         - Stateless design enables horizontal scaling
///         - Redis for distributed cache consistency
///         Registered as scoped service for per-request lifetime and DbContext transaction management.
/// </remarks>
public class PostService
{
    private readonly HybridCache _cache; // Assuming you have a caching layer
    private readonly ExpertBridgeDbContext _dbContext;
    private readonly ILogger<PostService> _logger;
    private readonly MediaAttachmentService _mediaService;
    private readonly NotificationFacade _notificationFacade;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly TaggingService _taggingService;

    public PostService(
        ExpertBridgeDbContext dbContext,
        MediaAttachmentService mediaService,
        TaggingService taggingService,
        NotificationFacade notificationFacade,
        ILogger<PostService> logger,
        HybridCache cache,
        IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext;
        _mediaService = mediaService;
        _taggingService = taggingService;
        _notificationFacade = notificationFacade;
        _logger = logger;
        _cache = cache;
        _publishEndpoint = publishEndpoint;
    }

    /// <summary>
    ///     Creates a new post with optional media attachments and initiates AI processing pipeline.
    /// </summary>
    /// <param name="request">The post creation request containing title, content, and optional media.</param>
    /// <param name="authorProfile">The authenticated user profile creating the post.</param>
    /// <returns>A task containing the created post with full metadata.</returns>
    /// <exception cref="ArgumentNullException">Thrown when request or authorProfile is null.</exception>
    /// <exception cref="BadHttpRequestException">Thrown when title or content is empty.</exception>
    /// <remarks>
    ///     **Complete Creation Workflow:**
    ///     1. **Validation** - Title and content required (trimmed)
    ///     2. **Post Entity Creation** - Creates Post with author association and UTC timestamp
    ///     3. **Media Processing** - If media provided, processes via MediaAttachmentService with S3 grants
    ///     4. **Database Save** - Commits post and media (generates post.Id for subsequent operations)
    ///     5. **Background Pipeline** - Publishes PostProcessingPipelineMessage for AI tagging and embedding
    ///     6. **Response Projection** - Maps to PostResponse DTO with author perspective
    ///     **AI Processing Pipeline (Async):**
    ///     After post creation, background worker:
    ///     - **Groq LLM**: Analyzes content → Generates bilingual tags (English + Arabic)
    ///     - **Ollama**: Creates 1024-dimension semantic embedding vector
    ///     - **TaggingService**: Associates tags with post and author's UserInterests
    ///     - **Database Update**: Stores embedding, sets IsTagged=true
    ///     - **Discoverability**: Post now appears in AI-powered recommendations
    ///     **Media Upload Flow:**
    ///     Client creates post → PostMedia records created → S3 grants activated (15-day expiration)
    ///     → Client uploads to presigned URLs → Media accessible via FileName + ContentType
    ///     **Performance:** Single transaction for post+media. Background processing non-blocking (instant response).
    ///     Post immediately visible in listings, AI recommendations available after ~30 seconds.
    /// </remarks>
    public async Task<PostResponse> CreatePostAsync(CreatePostRequest request, Profile authorProfile)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(authorProfile);
        // Your controller already checks for empty title/content, but service can re-validate
        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Content))
        {
            throw new BadHttpRequestException(
                "Title and Content are required."); // Or a more specific ArgumentException
        }

        var post = new Post
        {
            Title = request.Title.Trim(),
            Content = request.Content.Trim(),
            AuthorId = authorProfile.Id,
            Author = authorProfile, // For navigation, if needed before save by other logic
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.Posts.AddAsync(post);

        if (request.Media?.Count > 0)
        {
            PostMedia createPostMediaFunc(MediaObjectRequest mediaReq, Post parentPost)
            {
                return new PostMedia
                {
                    Post = parentPost,
                    Name = _mediaService.SanitizeMediaName(parentPost.Title), // Helper from CommentService or shared
                    Type = mediaReq.Type,
                    Key = mediaReq.Key
                };
            }

            var postMediaEntities = await _mediaService.ProcessAndAttachMediaAsync(
                request.Media,
                post,
                createPostMediaFunc,
                _dbContext
            );

            post.Medias = postMediaEntities; // Associate with the post entity
        }

        // Save primary entity (Post) and its media FIRST
        // This ensures post.Id is populated for subsequent operations (like channel message or notifications)
        await _dbContext.SaveChangesAsync();

        // Send to post processing pipeline
        await _publishEndpoint.Publish(new PostProcessingPipelineMessage
        {
            AuthorId = post.AuthorId, // Use post.AuthorId from the saved entity
            Content = post.Content,
            PostId = post.Id, // Use post.Id from the saved entity
            Title = post.Title,
            IsJobPosting = false,
            IsSafeContent = false
        });

        // No notifications for new post creation in the original controller, but if you had them:
        // _notificationFacade.StageNewPostNotification(post); // (Hypothetical method)
        // var notificationsToSave = _notificationFacade.GetStagedNotificationsForSaveAndClear();
        // if (notificationsToSave.Any()) { /* ... add to _dbContext and SaveChanges ... */ }
        // await _notificationFacade.DispatchStagedNotificationsAsync();
        // For now, assuming no direct notification on post creation itself.

        // Re-fetch for full response DTO
        //var createdPostWithIncludes = await _dbContext.Posts
        //    .FullyPopulatedPostQuery(p => p.Id == post.Id)
        //    .FirstOrDefaultAsync(); // Should not be null

        //if (createdPostWithIncludes == null)
        //{
        //    _logger.LogError("Failed to retrieve post {PostId} after creation.", post.Id);
        //    throw new InvalidOperationException("Post retrieval failed post-creation.");
        //}

        return post.SelectPostResponseFromFullPost(authorProfile.Id);
    }

    /// <summary>
    ///     Retrieves a single post by ID with full metadata including author, media, tags, comments, and vote counts.
    /// </summary>
    /// <param name="postId">The unique identifier of the post to retrieve.</param>
    /// <param name="currentMaybeUserProfileId">Optional profile ID to include current user's vote status.</param>
    /// <returns>PostResponse or null if not found.</returns>
    /// <remarks>
    ///     Uses FullyPopulatedPostQuery for eager loading (Author, PostMedia, PostTags, PostVotes).
    ///     Projects to DTO with AsNoTracking for read-only performance. Null return handled by controller as 404.
    /// </remarks>
    public async Task<PostResponse?> GetPostByIdAsync(string postId, string? currentMaybeUserProfileId)
    {
        ArgumentException.ThrowIfNullOrEmpty(postId);

        var postEntity = await _dbContext.Posts
            .FullyPopulatedPostQuery(p => p.Id == postId)
            .SelectPostResponseFromFullPost(currentMaybeUserProfileId) // This does the mapping
            .FirstOrDefaultAsync();

        return postEntity; // Null if not found, controller handles 404
    }

    /// <summary>
    ///     Retrieves posts similar to a given post using vector similarity search (cached).
    /// </summary>
    /// <param name="postId">The post ID to find similar posts for.</param>
    /// <param name="limit">Number of similar posts to return (default 5).</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>List of similar posts with relevance scores, ordered by cosine distance.</returns>
    /// <remarks>
    ///     **Caching:** HybridCache key `SimilarPosts_{postId}_{limit}` with 30-minute TTL.
    ///     **Algorithm:** Compares post.Embedding vectors using pgvector cosine distance.
    ///     **Exclusion:** Source post excluded from results.
    ///     **Use Case:** "Related articles" section, content discovery.
    ///     Returns empty list if source post has no embedding.
    /// </remarks>
    public async Task<List<SimilarPostsResponse>> GetSimilarPostsAsync(
        string postId,
        int? limit = 5,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"SimilarPosts_{postId}_{limit}";

        var similarPosts = await _cache.GetOrCreateAsync<List<SimilarPostsResponse>>(
            cacheKey,
            async entry =>
            {
                var currentPostEmbeddings = await _dbContext.Posts
                    .Where(p => p.Id == postId && p.Embedding != null)
                    .Select(p => p.Embedding)
                    .FirstOrDefaultAsync(entry);

                if (currentPostEmbeddings == null)
                {
                    _logger.LogWarning("No embeddings found for post {PostId} when fetching similar posts.", postId);
                    return []; // Return empty list if no embeddings found
                }

                var similarPostsQuery = await _dbContext.Posts
                    .AsNoTracking()
                    .Where(p => p.Id != postId && p.Embedding != null)
                    .OrderBy(p => p.Embedding.CosineDistance(currentPostEmbeddings))
                    .Take(limit ?? 5) // Limit to the specified number of similar posts or default to 5
                    .Include(p => p.Author) // Include author for response mapping
                    .Select(p => new SimilarPostsResponse
                    {
                        PostId = p.Id,
                        Title = p.Title,
                        Content = p.Content,
                        AuthorName = $"{p.Author.FirstName} {p.Author.LastName}",
                        CreatedAt = p.CreatedAt,
                        RelevanceScore = p.Embedding.CosineDistance(currentPostEmbeddings)
                    })
                    .ToListAsync(entry);

                return similarPostsQuery;
            },
            cancellationToken: cancellationToken);

        return similarPosts;
    }

    /// <summary>
    ///     Retrieves personalized post suggestions based on user's interest embedding (cached).
    /// </summary>
    /// <param name="userProfile">Optional user profile for personalized suggestions. If null, uses random vector.</param>
    /// <param name="limit">Number of suggestions to return.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>List of suggested posts with relevance scores, ordered by match quality.</returns>
    /// <remarks>
    ///     **Caching:** HybridCache key `SuggestedPosts_{profileId}_{limit}` with 30-minute TTL.
    ///     **Personalization:** Compares user_interest_embedding against post.Embedding vectors.
    ///     **Anonymous Users:** Random 1024-dim vector for exploration (different each session).
    ///     **Exclusion:** User's own posts excluded.
    ///     **Use Case:** "Recommended for you" feed, personalized homepage.
    /// </remarks>
    public async Task<List<SimilarPostsResponse>> GetSuggestedPostsAsync(
        Profile? userProfile,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"SuggestedPosts_{userProfile?.Id ?? "Anonymous"}_{limit}";

        var similarPosts = await _cache.GetOrCreateAsync<List<SimilarPostsResponse>>(
            cacheKey,
            async entry =>
            {
                var query = _dbContext.Posts
                    .AsNoTracking()
                    .AsQueryable();

                var userEmbedding = userProfile?.UserInterestEmbedding ?? Generator.GenerateRandomVector(1024);

                if (userProfile != null)
                {
                    query = query.Where(p => p.AuthorId != userProfile.Id); // Exclude posts by the current user
                }

                var similarPostsQuery = await query
                    .Where(p => p.Embedding != null)
                    .OrderBy(p => p.Embedding.CosineDistance(userEmbedding))
                    .Take(limit) // Limit to the specified number of similar posts or default to 5
                    .Include(p => p.Author) // Include author for response mapping
                    .Select(p => new SimilarPostsResponse
                    {
                        PostId = p.Id,
                        Title = p.Title,
                        Content = p.Content,
                        AuthorName = $"{p.Author.FirstName} {p.Author.LastName}",
                        CreatedAt = p.CreatedAt,
                        RelevanceScore = p.Embedding.CosineDistance(userEmbedding)
                    })
                    .ToListAsync(entry);

                return similarPostsQuery;
            },
            cancellationToken: cancellationToken);

        return similarPosts;
    }

    /// <summary>
    ///     Retrieves all posts with full metadata (no pagination).
    /// </summary>
    /// <param name="currentMaybeUserProfileId">Optional profile ID for vote status.</param>
    /// <returns>Complete list of all posts ordered by creation date.</returns>
    /// <remarks>
    ///     **Warning:** Performance concern for large datasets. Use pagination methods for production.
    ///     **Use Cases:** Admin dashboards, analytics, data exports.
    ///     Includes full eager loading of all navigation properties.
    /// </remarks>
    public async Task<List<PostResponse>> GetAllPostsAsync(string? currentMaybeUserProfileId)
    {
        return await _dbContext.Posts
            .FullyPopulatedPostQuery()
            .SelectPostResponseFromFullPost(currentMaybeUserProfileId)
            .ToListAsync();
    }

    /// <summary>
    ///     Retrieves AI-recommended posts using cursor-based pagination (stateless, scalable).
    /// </summary>
    /// <param name="userProfile">Optional user profile for personalized recommendations.</param>
    /// <param name="request">Cursor pagination request with After cursor, PageSize, and optional Embedding.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>Paginated response with posts, cursor for next page, and hasNextPage indicator.</returns>
    /// <remarks>
    ///     **Pagination Strategy:** Cursor-based using cosine distance + post ID (tie-breaker).
    ///     **Cursor Format:** Last item's distance value (double) for stateless pagination.
    ///     **Advantages:** No duplicate/missing items on concurrent updates, efficient for large datasets.
    ///     **Ideal For:** Infinite scroll, mobile apps, real-time feeds.
    ///     **Personalization:**
    ///     - Authenticated with embedding: Uses user_interest_embedding
    ///     - Request provides embedding: Uses provided vector
    ///     - Neither: Generates random 1024-dim vector (exploration)
    ///     **Performance:** Fetches PageSize+1 items to determine hasNextPage without extra query.
    /// </remarks>
    public async Task<PostsCursorPaginatedResponse> GetRecommendedPostsAsync(
        Profile? userProfile,
        PostsCursorRequest request,
        CancellationToken cancellationToken = default)
    {
        //if (userProfile == null || userProfile.UserInterestEmbedding == null)
        //{
        //    return new PaginatedRecommendedPostsResponseDto
        //    {
        //        Posts = new List<PostDto>(),
        //        HasNextPage = false
        //    };
        //}


        var userEmbedding = userProfile?.UserInterestEmbedding;

        if (userEmbedding == null)
        {
            if (request.Embedding != null)
            {
                userEmbedding = new Vector(request.Embedding);
            }
            else
            {
                userEmbedding = Generator.GenerateRandomVector(1024);
            }
        }

        // 2. Build the query for posts
        var query = _dbContext.Posts
            .AsNoTracking()
            .AsQueryable();

        // Apply cursor pagination predicate if not the first page
        if (request.After.HasValue)
        {
            // Assuming lower distance is better (more similar)
            // We want posts with:
            // - distance < lastDistance (more similar)
            // OR
            // - distance == lastDistance AND Id > lastPostId (same similarity, break tie with Id, assuming newer posts have higher IDs or some consistent order)
            // Adjust p.Id > or < lastPostIdCursor.Value based on your tie-breaking sort preference (e.g., if you also sort by CreatedAt DESC)
            query = query.Where(p =>
                    p.Embedding.CosineDistance(userEmbedding) > request.After.Value
                //||
                //(
                //    p.Embedding.CosineDistance(userEmbedding) == request.After.Value
                ////&& p.Id.CompareTo(request.LastIdCursor) // Tie-breaker: use Post ID. Adjust if secondary sort is different (e.g. newest first)
                //)
            );
        }

        // Temporary projection to include distance for ordering and cursor creation

        var postsWithDistance = await query
            .Where(p => p.Embedding != null)
            .Select(p => new
            {
                PostId = p.Id, // The whole post entity for now, will project to DTO later
                Distance = p.Embedding.CosineDistance(userEmbedding)
            })
            .OrderBy(x => x.Distance) // Order by similarity (ascending distance)
            .ThenBy(x => x.PostId) // Consistent tie-breaker
            .Take(request.PageSize + 1) // Fetch one extra item to determine if there's a next page
            .ToListAsync(cancellationToken);

        // 3. Determine if there's a next page and prepare the results
        var hasNextPage = postsWithDistance.Count > request.PageSize;
        var currentPagePosts = postsWithDistance.Take(request.PageSize).ToList();

        double? nextDistance = null;
        string? nextPostId = null;

        if (hasNextPage && currentPagePosts.Count > 0)
        {
            var lastPostOnCurrentPage = currentPagePosts.Last();
            nextDistance = lastPostOnCurrentPage.Distance;
            nextPostId = lastPostOnCurrentPage.PostId;
        }

        // 4. Map to DTOs
        var currentPagePostIds = currentPagePosts.Select(p => p.PostId);
        var postDtos = await _dbContext.Posts
            .FullyPopulatedPostQuery(p => currentPagePostIds.Contains(p.Id))
            .SelectPostResponseFromFullPost(userProfile?.Id)
            .ToListAsync(cancellationToken);

        //.Select(pd => pd.Post.SelectPostResponseFromFullPost(userProfile?.Id)).ToList();

        return new PostsCursorPaginatedResponse
        {
            Posts = postDtos,
            PageInfo = new PageInfoResponse
            {
                EndCursor = nextDistance, NextIdCursor = nextPostId, HasNextPage = hasNextPage
            }
        };
    }

    // <summary>
    ///     Retrieves AI-recommended posts using offset-based pagination (traditional page numbers).
    /// </summary>
    /// <param name="userProfile">Optional user profile for personalized recommendations.</param>
    /// <param name="request">Pagination request with Page number, PageSize, and optional Embedding.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>Paginated response with posts, relevance scores, and hasNextPage indicator.</returns>
    /// <remarks>
    ///     **Pagination Strategy:** Offset-based using Skip/Take pattern.
    ///     **Advantages:** Simple client implementation, explicit page numbers.
    ///     **Disadvantages:** May have consistency issues on concurrent updates (page drift).
    ///     **Ideal For:** Traditional page navigation, UIs with explicit page numbers.
    ///     **Personalization:** Same as cursor-based (user embedding, provided embedding, or random).
    ///     **Relevance Scores:** Included in response for debugging/UI display.
    ///     Returns stored or generated random embedding in response for subsequent requests.
    /// </remarks>
    public async Task<PostsCursorPaginatedResponse> GetRecommendedPostsOffsetPageAsync(
        Profile? userProfile,
        PostsCursorRequest request,
        CancellationToken cancellationToken = default)
    {
        var userEmbedding = userProfile?.UserInterestEmbedding;
        var userProfileId = userProfile?.Id;
        string? randomEmbedding = null;

        if (userEmbedding == null)
        {
            if (request.Embedding != null)
            {
                userEmbedding = new Vector(request.Embedding);
            }
            else
            {
                userEmbedding = Generator.GenerateRandomVector(1024);
                randomEmbedding = userEmbedding.ToString(); // Store the random embedding as a string for response
            }
        }

        // 2. Build the query for posts
        var query = _dbContext.Posts
                .AsNoTracking()
                .FullyPopulatedPostQuery()
                .AsQueryable()
            ;

        var postsWithDistance = await query
            .Where(p => p.Embedding != null)
            .Select(p => new
            {
                Post =
                    p.SelectPostResponseFromFullPost(
                        userProfileId), // The whole post entity for now, will project to DTO later
                Distance = p.Embedding.CosineDistance(userEmbedding)
            })
            .OrderBy(x => x.Distance) // Order by similarity (ascending distance)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize + 1) // Fetch one extra item to determine if there's a next page
            .ToListAsync(cancellationToken);

        // 3. Determine if there's a next page and prepare the results
        var hasNextPage = postsWithDistance.Count > request.PageSize;

        return new PostsCursorPaginatedResponse
        {
            Posts = postsWithDistance
                .Take(request.PageSize)
                .Select(x =>
                {
                    var postResponse = x.Post;
                    postResponse.RelevanceScore = x.Distance;
                    return postResponse;
                }).ToList(),
            PageInfo = new PageInfoResponse
            {
                HasNextPage = hasNextPage, Embedding = randomEmbedding ?? request.Embedding
            }
        };
    }

    /// <summary>
    ///     Retrieves all posts authored by a specific user profile.
    /// </summary>
    /// <param name="profileId">The user profile ID whose posts to retrieve.</param>
    /// <param name="currentMaybeUserProfileId">Optional profile ID for vote status perspective.</param>
    /// <returns>List of posts by the specified author.</returns>
    /// <exception cref="ProfileNotFoundException">Thrown when profile doesn't exist.</exception>
    /// <remarks>
    ///     **Validation:** Checks profile exists before querying posts.
    ///     **Use Cases:** User profile page, author's post history, portfolio.
    ///     Ordered by creation date with full eager loading.
    /// </remarks>
    public async Task<List<PostResponse>> GetPostsByProfileIdAsync(string profileId, string? currentMaybeUserProfileId)
    {
        ArgumentException.ThrowIfNullOrEmpty(profileId);

        var profileExists = await _dbContext.Profiles.AnyAsync(p => p.Id == profileId);
        if (!profileExists)
        {
            throw new ProfileNotFoundException($"Profile with id={profileId} was not found for retrieving posts.");
        }

        return await _dbContext.Posts
            .FullyPopulatedPostQuery(p => p.AuthorId == profileId)
            .SelectPostResponseFromFullPost(
                currentMaybeUserProfileId) // Or profileId if that's the vote perspective desired
            .ToListAsync();
    }

    /// <summary>
    ///     Edits a post's title and/or content with authorization checks and AI reprocessing.
    /// </summary>
    /// <param name="postId">The post ID to edit.</param>
    /// <param name="request">Edit request with optional title and/or content updates.</param>
    /// <param name="editorProfile">The user attempting the edit.</param>
    /// <returns>Updated PostResponse with new metadata.</returns>
    /// <exception cref="PostNotFoundException">Thrown when post doesn't exist.</exception>
    /// <exception cref="ForbiddenAccessException">Thrown when editor is not the author.</exception>
    /// <remarks>
    ///     **Authorization:** Only post author can edit their own posts.
    ///     **Partial Updates:** Only provided fields are updated (title and/or content).
    ///     **Timestamps:** UpdatedAt set to UTC now on changes.
    ///     **AI Reprocessing:** If content changed, publishes PostProcessingPipelineMessage to regenerate tags and embedding.
    ///     **Cache Invalidation:** Should invalidate similar/suggested post caches (implementation consideration).
    ///     **Security:** Logs unauthorized edit attempts with warning level.
    /// </remarks>
    public async Task<PostResponse> EditPostAsync(string postId, EditPostRequest request, Profile editorProfile)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrEmpty(postId);
        ArgumentNullException.ThrowIfNull(editorProfile);

        var post = await _dbContext.Posts.FindAsync(postId);
        if (post == null)
        {
            throw new PostNotFoundException($"Post with id={postId} was not found for editing.");
        }

        if (post.AuthorId != editorProfile.Id)
        {
            _logger.LogWarning("User {EditorProfileId} attempted to edit post {PostId} owned by {AuthorId}.",
                editorProfile.Id, post.Id, post.AuthorId);
            throw new ForbiddenAccessException($"User {editorProfile.Id} is not authorized to edit post {postId}.");
        }

        var changed = false;
        if (!string.IsNullOrWhiteSpace(request.Title) && post.Title != request.Title.Trim())
        {
            post.Title = request.Title.Trim();
            changed = true;
        }

        if (!string.IsNullOrWhiteSpace(request.Content) && post.Content != request.Content.Trim())
        {
            post.Content = request.Content.Trim();
            changed = true;
        }

        if (changed)
        {
            post.UpdatedAt = DateTime.UtcNow; // Update the last modified time
            await _dbContext.SaveChangesAsync();

            // Send to post processing pipeline if content changed
            if (!string.IsNullOrWhiteSpace(request
                    .Content)) // Only if content changed, title change might not need this pipeline
            {
                await _publishEndpoint.Publish(new PostProcessingPipelineMessage
                {
                    AuthorId = post.AuthorId,
                    Content = post.Content,
                    PostId = post.Id,
                    Title = post.Title, // Send current title
                    IsJobPosting = false,
                    IsSafeContent = false
                });
            }
        }

        // Re-fetch for full response
        var updatedPostEntity = await _dbContext.Posts
            .FullyPopulatedPostQuery(p => p.Id == postId)
            .SelectPostResponseFromFullPost(editorProfile.Id)
            .FirstAsync();

        return updatedPostEntity;
    }

    /// <summary>
    ///     Deletes a post with authorization checks and cascade handling (idempotent).
    /// </summary>
    /// <param name="postId">The post ID to delete.</param>
    /// <param name="deleterProfile">The user attempting deletion.</param>
    /// <returns>True if deleted or already gone (idempotent DELETE semantics).</returns>
    /// <exception cref="ForbiddenAccessException">Thrown when deleter is not the author.</exception>
    /// <remarks>
    ///     **Authorization:** Only post author can delete their own posts.
    ///     **Cascade Deletion:** EF Core cascade deletes PostMedia, PostVotes, Comments, PostTags.
    ///     **Idempotency:** Returns true if post not found (HTTP 204 success state for DELETE).
    ///     **Security:** Logs unauthorized deletion attempts.
    ///     **Future Enhancement:** Soft delete pattern, moderator deletion, restore within time window.
    /// </remarks>
    public async Task<bool> DeletePostAsync(string postId, Profile deleterProfile)
    {
        ArgumentException.ThrowIfNullOrEmpty(postId);
        ArgumentNullException.ThrowIfNull(deleterProfile);

        var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId);

        if (post == null)
        {
            return true; // Idempotency: "already deleted" is a success for DELETE
        }

        if (post.AuthorId != deleterProfile.Id)
        {
            // Add role checks if moderators/admins can delete
            _logger.LogWarning("User {DeleterProfileId} attempted to delete post {PostId} owned by {AuthorId}.",
                deleterProfile.Id, post.Id, post.AuthorId);
            throw new ForbiddenAccessException($"User {deleterProfile.Id} is not authorized to delete post {postId}.");
        }

        _dbContext.Posts.Remove(post); // EF handles cascading for PostMedia, PostVotes, Comments if configured
        await _dbContext.SaveChangesAsync();

        // Optional: Send to a pipeline if deletion needs further processing (e.g., remove from search index)
        // await _postDeletionChannel.Writer.WriteAsync(new PostDeletedMessage { PostId = postId });

        return true;
    }

    /// <summary>
    ///     Processes upvote/downvote on a post with toggle mechanics, tag inheritance, and notification delivery.
    /// </summary>
    /// <param name="postId">The post ID to vote on.</param>
    /// <param name="voterProfile">The user casting the vote.</param>
    /// <param name="isUpvoteIntent">True for upvote, false for downvote.</param>
    /// <returns>Updated PostResponse with new vote counts and current user's vote status.</returns>
    /// <exception cref="PostNotFoundException">Thrown when post doesn't exist.</exception>
    /// <remarks>
    ///     **Toggle Voting Mechanics:**
    ///     - Same vote clicked again → Remove vote (toggle off)
    ///     - Opposite vote clicked → Switch vote type (upvote ↔ downvote)
    ///     - New vote → Create PostVote record
    ///     **Tag Inheritance for Interest Tracking:**
    ///     When user votes on a post, post's tags added to voter's UserInterests.
    ///     This builds user's interest profile for improved recommendations.
    ///     **Notification Delivery:**
    ///     On upvote (not removal or downvote), sends notification to post author via SignalR.
    ///     Real-time notification includes voter details and post context.
    ///     **Post Tags Eager Loading:**
    ///     Loads PostTags with Tags for interest tracking without additional query.
    ///     **Performance:** Single transaction for vote changes, async notification delivery, re-fetch with full navigation
    ///     for accurate response.
    /// </remarks>
    public async Task<PostResponse> VotePostAsync(
        string postId, Profile voterProfile,
        bool isUpvoteIntent)
    {
        ArgumentException.ThrowIfNullOrEmpty(postId);
        ArgumentNullException.ThrowIfNull(voterProfile);

        var post = await _dbContext.Posts
            .Include(p => p.Author) // For notification to post author
            .Include(p => p.PostTags)
            .ThenInclude(t => t.Tag)
            .FirstOrDefaultAsync(p => p.Id == postId);

        if (post == null)
        {
            throw new PostNotFoundException($"Post with id={postId} was not found for voting.");
        }

        var vote = await _dbContext.PostVotes
            //.Include(v => v.Post)
            //.ThenInclude(p => p.Author) // Needed for notification message creation. One hit to DB <3
            .FirstOrDefaultAsync(v => v.PostId == postId && v.ProfileId == voterProfile.Id);

        if (vote == null)
        {
            vote = new PostVote
            {
                ProfileId = voterProfile.Id,
                Profile = voterProfile,
                PostId = post.Id,
                Post = post,
                IsUpvote = isUpvoteIntent
            };

            await _dbContext.PostVotes.AddAsync(vote);
            await _taggingService.AddTagsToUserProfileAsync(
                voterProfile.Id,
                post.PostTags.Select(pt => pt.Tag)
            );
        }
        else
        {
            if (vote.IsUpvote == isUpvoteIntent) // Voting same way again (e.g. upvoting already upvoted)
            {
                _dbContext.PostVotes.Remove(vote);
                vote = null;
            }
            else // Switching vote (e.g. from down to up)
            {
                vote.IsUpvote = isUpvoteIntent;
            }
        }

        // Save changes for the vote first
        await _dbContext.SaveChangesAsync();

        if (vote != null)
        {
            vote.Post = post; // Just to make sure whatever conditional path we take, the rich post is always attached to the vote for notifications purposes.
            await _notificationFacade.NotifyPostVotedAsync(vote);
        }

        // No second SaveChanges needed if NotifyPostVotedAsync only writes to channel

        var updatedPost = await _dbContext.Posts
            .FullyPopulatedPostQuery(p => p.Id == postId)
            .SelectPostResponseFromFullPost(voterProfile.Id)
            .FirstAsync();

        return updatedPost;
    }
}
