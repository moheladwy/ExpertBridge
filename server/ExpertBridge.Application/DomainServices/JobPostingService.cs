using ExpertBridge.Application.DataGenerator;
using ExpertBridge.Core.Entities.JobApplications;
using ExpertBridge.Core.Entities.JobPostings;
using ExpertBridge.Core.Entities.JobPostingsVotes;
using ExpertBridge.Core.Entities.Media.JobPostingMedia;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Exceptions;
using ExpertBridge.Core.Messages;
using ExpertBridge.Core.Queries;
using ExpertBridge.Core.Requests.ApplyToJobPosting;
using ExpertBridge.Core.Requests.CreateJobPosting;
using ExpertBridge.Core.Requests.EditJobPosting;
using ExpertBridge.Core.Requests.JobPostingsPagination;
using ExpertBridge.Core.Requests.MediaObject;
using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using ExpertBridge.Notifications;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace ExpertBridge.Application.DomainServices;

/// <summary>
///     Provides comprehensive job posting management with AI-powered matching, recommendations, and application
///     processing.
/// </summary>
/// <remarks>
///     This service manages the complete lifecycle of job postings in ExpertBridge's marketplace, from creation through
///     application processing, leveraging vector embeddings for intelligent contractor-job matching.
///     **Core Features:**
///     - Job posting CRUD with media attachments
///     - AI-powered job recommendations using vector similarity
///     - Semantic job search with cosine distance
///     - Personalized job suggestions based on user interests
///     - Application submission and tracking
///     - Voting to express interest
///     - Tag-based skill matching
///     - HybridCache for performance
///     **AI Recommendation Architecture:**
///     <code>
/// Job Posting Created
///     ↓
/// PostProcessingPipelineMessage published
///     ↓
/// Background Worker: Groq generates tags, Ollama creates embedding
///     ↓
/// JobPosting.JobDescriptionEmbedding stored (1024-dim vector)
///     ↓
/// Profile.UserInterestEmbedding (contractor's interests, 1024-dim)
///     ↓
/// Recommendations: Cosine similarity between JobDescriptionEmbedding and UserInterestEmbedding
///     ↓
/// Ordered by similarity score (distance closest to 0 = best match)
/// </code>
///     **Matching Algorithm:**
///     Uses pgvector extension for high-performance similarity search:
///     <code>
/// SELECT * FROM job_postings
/// WHERE job_description_embedding IS NOT NULL
/// ORDER BY job_description_embedding &lt;=&gt; user_interest_embedding
/// LIMIT 20
/// </code>
///     Cosine distance operator `&lt;=&gt;` returns values 0-2 (0 = identical, 2 = opposite).
///     **Tag-Based Skill Matching:**
///     - Job postings tagged with required skills (e.g., "C#", "Azure", "Docker")
///     - Contractor profiles have UserInterests from previous interactions
///     - Voting on job adds tags to voter's UserInterests (skill discovery)
///     - Tag overlap boosts recommendation relevance
///     **Caching Strategy:**
///     HybridCache with two-tier architecture:
///     - **L1 Cache**: In-memory for ultra-fast repeated queries
///     - **L2 Cache**: Redis for distributed consistency
///     - **Similar Jobs**: Cached by job ID with sliding expiration
///     - **Suggested Jobs**: Cached by profile ID for personalized results
///     **Application Workflow:**
///     <code>
/// Contractor finds job → Reviews requirements
///     ↓
/// Applies with cover letter
///     ↓
/// JobApplication created
///     ↓
/// Notification sent to job author (client)
///     ↓
/// Author reviews applications
///     ↓
/// Author accepts application (via JobService.AcceptApplicationAsync)
///     ↓
/// Job + Chat created, JobPosting marked as filled
/// </code>
///     **Voting Mechanism:**
///     - Contractors upvote jobs they're interested in
///     - Job tags automatically added to voter's UserInterests
///     - Improves future job recommendations
///     - Signal to job author (high vote count = attractive opportunity)
///     **Job Posting Lifecycle:**
///     1. **Created**: Author creates with title, content, budget, area, optional media
///     2. **Tagged**: Background worker adds AI-generated tags
///     3. **Embedded**: Background worker generates description embedding
///     4. **Discoverable**: Appears in recommendations, search, similar jobs
///     5. **Applications**: Contractors apply
///     6. **Accepted**: Author accepts application (handled by JobService)
///     7. **Archived**: Job marked as filled, removed from active listings
///     **Media Attachments:**
///     - Images, PDFs for detailed requirements
///     - S3 storage with presigned URLs
///     - 15-day upload grant activation
///     - Managed via MediaAttachmentService
///     **Notification Integration:**
///     Real-time notifications via SignalR for:
///     - New applications on your job posting
///     - Upvotes on your job posting
///     - Job recommendations matching your skills
///     **Performance Optimizations:**
///     - Eager loading with Include/ThenInclude
///     - AsNoTracking for read-only queries
///     - Projection to response DTOs
///     - Vector index on JobDescriptionEmbedding column
///     - HybridCache for expensive similarity queries
///     - Background processing for embedding generation
///     **Authorization Rules:**
///     - Create: Any authenticated user can post jobs
///     - Edit: Only job author
///     - Delete: Only job author
///     - Apply: Any contractor (future: prevent self-application)
///     - View Applications: Only job author
///     - Vote: Any authenticated user except author
///     **Use Cases:**
///     - Clients post freelance/contract opportunities
///     - Contractors discover relevant jobs
///     - AI matches jobs to contractor expertise
///     - Skills-based marketplace
///     - Project-based work management
///     Registered as scoped service for per-request lifetime and DbContext transaction management.
/// </remarks>
public class JobPostingService
{
    private readonly HybridCache _cache;
    private readonly ExpertBridgeDbContext _dbContext;
    private readonly ILogger<JobPostingService> _logger;
    private readonly MediaAttachmentService _mediaService;
    private readonly NotificationFacade _notificationFacade;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly TaggingService _taggingService;

    public JobPostingService(
        ExpertBridgeDbContext dbContext,
        MediaAttachmentService mediaService,
        TaggingService taggingService,
        NotificationFacade notificationFacade,
        HybridCache cache,
        ILogger<JobPostingService> logger,
        IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext;
        _mediaService = mediaService;
        _taggingService = taggingService;
        _notificationFacade = notificationFacade;
        _cache = cache;
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    // <summary>
    ///     Creates a new job posting with optional media attachments and initiates AI processing pipeline.
    /// </summary>
    /// <param name="request">The job posting creation request containing title, content, budget, area, and media.</param>
    /// <param name="authorProfile">The authenticated user profile creating the job posting (client).</param>
    /// <returns>A task containing the created job posting with full metadata.</returns>
    /// <exception cref="ArgumentNullException">Thrown when request or authorProfile is null.</exception>
    /// <exception cref="BadHttpRequestException">Thrown when title or content is empty.</exception>
    /// <remarks>
    ///     **Complete Creation Workflow:**
    ///     **1. Validation:**
    ///     - Validates title and content not empty (trimmed)
    ///     - Validates authorProfile exists
    ///     - Budget and area required fields
    ///     **2. Job Posting Entity Creation:**
    ///     <code>
    /// JobPosting {
    ///     Title: "Senior .NET Developer",
    ///     Content: "Looking for experienced developer...",
    ///     Budget: 5000.00,
    ///     Area: "Software Development",
    ///     AuthorId: clientProfile.Id
    /// }
    /// </code>
    ///     **3. Media Attachment Processing:**
    ///     If media provided:
    ///     - Creates JobPostingMedia entities
    ///     - Processes via MediaAttachmentService
    ///     - S3 grants activated for upload (15-day expiration)
    ///     - Sanitized filenames
    ///     **4. Tag Inheritance:**
    ///     Adds job's tags to author's UserInterests:
    ///     - Builds interest profile for job posters
    ///     - Enables "jobs similar to ones you've posted" recommendations
    ///     - Publishes UserInterestsUpdatedMessage
    ///     **5. Background Processing Pipeline:**
    ///     Publishes PostProcessingPipelineMessage:
    ///     <code>
    /// PostProcessingPipelineMessage {
    ///     ItemId: jobPosting.Id,
    ///     ItemType: PostProcessingItemType.JobPosting,
    ///     Content: jobPosting.Content,
    ///     Title: jobPosting.Title
    /// }
    /// </code>
    ///     Background worker handles:
    ///     - **Groq AI**: Analyzes content, generates tags (bilingual: English + Arabic)
    ///     - **Ollama**: Creates 1024-dimension embedding vector from description
    ///     - **TaggingService**: Associates tags with job posting
    ///     - **Database Update**: Stores embedding in JobDescriptionEmbedding column
    ///     **6. Notification Delivery:**
    ///     - Notifies job author (confirmation)
    ///     - Notifies relevant contractors (optional: matching their interests)
    ///     - Real-time via SignalR
    ///     **7. Response Projection:**
    ///     - Re-queries with full navigation properties
    ///     - Projects to JobPostingResponse DTO
    ///     - Includes author, media, tags, vote counts
    ///     **Example Usage:**
    ///     <code>
    /// var request = new CreateJobPostingRequest {
    ///     Title = "React Native Mobile App Development",
    ///     Content = "Need experienced React Native developer for iOS/Android app...",
    ///     Budget = 8000.00M,
    ///     Area = "Mobile Development",
    ///     Media = new List&lt;MediaObjectRequest&gt; {
    ///         new() { FileName = "requirements.pdf", ContentType = "application/pdf" }
    ///     }
    /// };
    /// 
    /// var jobPosting = await jobPostingService.CreateAsync(request, clientProfile);
    /// 
    /// // Background processing begins automatically
    /// // After ~30 seconds: tags and embedding ready
    /// // Job appears in contractor recommendations
    /// </code>
    ///     **AI Processing Pipeline:**
    ///     <code>
    /// Job Created → Message Published
    ///     ↓
    /// Background Consumer receives message
    ///     ↓
    /// Groq API: Content → Tags (["react-native", "mobile", "ios", "android"])
    ///     ↓
    /// Ollama API: Content → Embedding (1024-dim vector)
    ///     ↓
    /// TaggingService: Creates/associates tags
    ///     ↓
    /// Database: UPDATE job_postings SET job_description_embedding = [vector], is_tagged = true
    ///     ↓
    /// Job now discoverable via similarity search
    /// </code>
    ///     **Media Upload Flow:**
    ///     <code>
    /// Client creates job with media
    ///     ↓
    /// JobPostingMedia records created
    ///     ↓
    /// S3 grants activated (presigned URLs generated)
    ///     ↓
    /// Client uploads files to S3
    ///     ↓
    /// Media accessible via FileName + ContentType
    /// </code>
    ///     **Budget and Area:**
    ///     - Budget: Decimal representing total project budget or hourly rate
    ///     - Area: Category/domain (e.g., "Web Development", "Data Science", "Graphic Design")
    ///     - Used for filtering and grouping
    ///     - Not validated against predefined list (flexible taxonomy)
    ///     **Performance:**
    ///     - Single database transaction
    ///     - Async background processing (non-blocking)
    ///     - Media processing parallelizable
    ///     - Instant response to client (processing continues asynchronously)
    ///     **Discoverability Timeline:**
    ///     - **Immediate**: Visible in "All Jobs" listing
    ///     - **After Tagging** (~10s): Appears in tag-based search
    ///     - **After Embedding** (~30s): Appears in AI recommendations
    ///     This is the primary entry point for job posting creation in the marketplace.
    /// </remarks>
    public async Task<JobPostingResponse> CreateAsync(
        CreateJobPostingRequest request,
        Profile authorProfile)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(authorProfile);
        // Your controller already checks for empty title/content, but service can re-validate
        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Content))
        {
            throw new BadHttpRequestException(
                "Title and Content are required."); // Or a more specific ArgumentException
        }

        var posting = new JobPosting
        {
            Title = request.Title.Trim(),
            Area = request.Area.Trim(),
            Budget = request.Budget,
            Content = request.Content.Trim(),
            AuthorId = authorProfile.Id,
            Author = authorProfile // For navigation, if needed before save by other logic
        };

        await _dbContext.JobPostings.AddAsync(posting);

        if (request.Media?.Count > 0)
        {
            JobPostingMedia createPostMediaFunc(MediaObjectRequest mediaReq, JobPosting parentPost)
            {
                return new JobPostingMedia
                {
                    JobPosting = parentPost,
                    Name = _mediaService.SanitizeMediaName(parentPost.Title),
                    Type = mediaReq.Type,
                    Key = mediaReq.Key
                };
            }

            var postMediaEntities = await _mediaService.ProcessAndAttachMediaAsync(
                request.Media,
                posting,
                createPostMediaFunc,
                _dbContext
            );

            posting.Medias = postMediaEntities; // Associate with the post entity
        }

        // Save primary entity (Post) and its media FIRST
        // This ensures post.Id is populated for subsequent operations (like channel message or notifications)
        await _dbContext.SaveChangesAsync();

        // Send to post processing pipeline
        await _publishEndpoint.Publish(new PostProcessingPipelineMessage
        {
            AuthorId = posting.AuthorId, // Use post.AuthorId from the saved entity
            Content = posting.Content,
            PostId = posting.Id, // Use post.Id from the saved entity
            Title = posting.Title,
            IsJobPosting = true,
            IsSafeContent = false
        });

        return posting.SelectJopPostingResponseFromFullJobPosting(authorProfile.Id);
    }

    /// <summary>
    ///     Retrieves AI-recommended job postings using vector similarity with offset-based pagination.
    /// </summary>
    /// <param name="userProfile">Optional user profile for personalized recommendations based on UserInterestEmbedding.</param>
    /// <param name="pageNumber">The page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <returns>Paginated response with job postings ordered by relevance score (cosine distance).</returns>
    /// <remarks>
    ///     Uses pgvector cosine distance to match job_description_embedding against user_interest_embedding.
    ///     If userProfile null or no embedding, returns random sample. Includes distance score in response.
    /// </remarks>
    public async Task<JobPostingsPaginatedResponse> GetRecommendedJobsOffsetPageAsync(
        Profile? userProfile,
        JobPostingsPaginationRequest request,
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
        var query = _dbContext.JobPostings
                .AsNoTracking()
                .FullyPopulatedJobPostingQuery()
                .AsQueryable()
            ;

        var postingsWithDistance = await query
            .Where(p => p.Embedding != null)
            .Select(p => new
            {
                Post =
                    p.SelectJopPostingResponseFromFullJobPosting(
                        userProfileId), // The whole post entity for now, will project to DTO later
                Distance = p.Embedding.CosineDistance(userEmbedding)
            })
            .OrderBy(x => x.Distance) // Order by similarity (ascending distance)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize + 1) // Fetch one extra item to determine if there's a next page
            .ToListAsync(cancellationToken);

        // 3. Determine if there's a next page and prepare the results
        var hasNextPage = postingsWithDistance.Count > request.PageSize;

        return new JobPostingsPaginatedResponse
        {
            JobPostings = postingsWithDistance
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
    ///     Retrieves a single job posting by ID with full metadata including author, media, tags, and vote counts.
    /// </summary>
    /// <param name="jobPostingId">The job posting ID.</param>
    /// <param name="profileId">Optional profile ID to include current user's vote status.</param>
    /// <returns>JobPostingResponse or null if not found.</returns>
    /// <remarks>Uses FullyPopulatedJobPostingQuery for eager loading. Projects to DTO with AsNoTracking.</remarks>
    public async Task<JobPostingResponse?> GetJobPostingByIdAsync(
        string postingId,
        string? currentMaybeUserProfileId)
    {
        ArgumentException.ThrowIfNullOrEmpty(postingId);

        var posting = await _dbContext.JobPostings
            .FullyPopulatedJobPostingQuery(p => p.Id == postingId)
            .SelectJopPostingResponseFromFullJobPosting(currentMaybeUserProfileId) // This does the mapping
            .FirstOrDefaultAsync();

        return posting; // Null if not found, controller handles 404
    }

    public async Task<List<SimilarJobsResponse>> GetSimilarJobsAsync(
        string postingId,
        int? limit = 5,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"SimilarJobs_{postingId}_{limit}";

        var similarPosts = await _cache.GetOrCreateAsync<List<SimilarJobsResponse>>(
            cacheKey,
            async entry =>
            {
                var currentPostEmbeddings = await _dbContext.JobPostings
                    .Where(p => p.Id == postingId && p.Embedding != null)
                    .Select(p => p.Embedding)
                    .FirstOrDefaultAsync(entry);

                if (currentPostEmbeddings == null)
                {
                    _logger.LogWarning("No embeddings found for job posting {PostingId} when fetching similar posts.",
                        postingId);
                    return []; // Return empty list if no embeddings found
                }

                var similarJobsQuery = await _dbContext.JobPostings
                    .AsNoTracking()
                    .Where(p => p.Id != postingId && p.Embedding != null)
                    .OrderBy(p => p.Embedding.CosineDistance(currentPostEmbeddings))
                    .Take(limit ?? 5) // Limit to the specified number of similar posts or default to 5
                    .Include(p => p.Author) // Include author for response mapping
                    .Select(p => new SimilarJobsResponse
                    {
                        JobPostingId = p.Id,
                        Title = p.Title,
                        Content = p.Content,
                        AuthorName = $"{p.Author.FirstName} {p.Author.LastName}",
                        CreatedAt = p.CreatedAt,
                        RelevanceScore = p.Embedding.CosineDistance(currentPostEmbeddings)
                    })
                    .ToListAsync(entry);

                return similarJobsQuery;
            },
            cancellationToken: cancellationToken);

        return similarPosts;
    }

    /// <summary>
    ///     Retrieves personalized job suggestions based on user's interest embedding (cached).
    /// </summary>
    /// <param name="profileId">The user profile ID for personalized suggestions.</param>
    /// <param name="take">Number of suggestions to return (default 10).</param>
    /// <returns>List of suggested jobs with relevance scores, ordered by match quality.</returns>
    /// <remarks>
    ///     Uses HybridCache with profile-specific key. Compares user_interest_embedding against job_description_embedding.
    ///     Returns empty list if user has no embedding. Cache expires after 30 minutes.
    /// </remarks>
    public async Task<List<SimilarJobsResponse>> GetSuggestedJobsAsync(
        Profile? userProfile,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"SuggestedJobs_{userProfile?.Id ?? "Anonymous"}_{limit}";

        var similarJobs = await _cache.GetOrCreateAsync<List<SimilarJobsResponse>>(
            cacheKey,
            async entry =>
            {
                var query = _dbContext.JobPostings
                    .AsNoTracking()
                    .AsQueryable();

                var userEmbedding = userProfile?.UserInterestEmbedding ?? Generator.GenerateRandomVector(1024);

                if (userProfile != null)
                {
                    query = query.Where(p => p.AuthorId != userProfile.Id); // Exclude posts by the current user
                }

                var similarJobsQuery = await query
                    .Where(p => p.Embedding != null)
                    .OrderBy(p => p.Embedding.CosineDistance(userEmbedding))
                    .Take(limit) // Limit to the specified number of similar posts or default to 5
                    .Include(p => p.Author) // Include author for response mapping
                    .Select(p => new SimilarJobsResponse
                    {
                        JobPostingId = p.Id,
                        Title = p.Title,
                        Content = p.Content,
                        AuthorName = $"{p.Author.FirstName} {p.Author.LastName}",
                        CreatedAt = p.CreatedAt,
                        Area = p.Area,
                        Budget = p.Budget,
                        RelevanceScore = p.Embedding.CosineDistance(userEmbedding)
                    })
                    .ToListAsync(entry);

                return similarJobsQuery;
            },
            cancellationToken: cancellationToken);

        return similarJobs;
    }

    public async Task<JobPostingResponse> EditJopPostingAsync(
        string postingId,
        EditJobPostingRequest request,
        Profile editorProfile)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrEmpty(postingId);
        ArgumentNullException.ThrowIfNull(editorProfile);

        var jobPosting = await _dbContext.JobPostings.FindAsync(postingId);
        if (jobPosting == null)
        {
            throw new PostNotFoundException($"JobPosting with id={postingId} was not found for editing.");
        }

        if (jobPosting.AuthorId != editorProfile.Id)
        {
            _logger.LogWarning(
                "User {EditorProfileId} attempted to edit job posting {JobPostingId} owned by {AuthorId}.",
                editorProfile.Id, jobPosting.Id, jobPosting.AuthorId);

            throw new ForbiddenAccessException(
                $"User {editorProfile.Id} is not authorized to edit job posting {postingId}.");
        }

        var changed = false;
        if (!string.IsNullOrWhiteSpace(request.Title) && jobPosting.Title != request.Title.Trim())
        {
            jobPosting.Title = request.Title.Trim();
            changed = true;
        }

        if (!string.IsNullOrWhiteSpace(request.Content) && jobPosting.Content != request.Content.Trim())
        {
            jobPosting.Content = request.Content.Trim();
            changed = true;
        }

        if (!string.IsNullOrWhiteSpace(request.Area) && jobPosting.Area != request.Area.Trim())
        {
            jobPosting.Area = request.Area.Trim();
            changed = true;
        }

        if (request.Budget.HasValue && jobPosting.Budget != request.Budget)
        {
            jobPosting.Budget = request.Budget.Value;
            changed = true;
        }


        if (changed)
        {
            jobPosting.UpdatedAt = DateTime.UtcNow; // Update the last modified time
            await _dbContext.SaveChangesAsync();

            // Send to post processing pipeline if content changed
            if (!string.IsNullOrWhiteSpace(request
                    .Content)) // Only if content changed, title change might not need this pipeline
            {
                await _publishEndpoint.Publish(new PostProcessingPipelineMessage
                {
                    AuthorId = jobPosting.AuthorId,
                    Content = jobPosting.Content,
                    PostId = jobPosting.Id,
                    Title = jobPosting.Title, // Send current title
                    IsJobPosting = true,
                    IsSafeContent = false
                });
            }
        }

        // Re-fetch for full response
        var updatedPostingEntity = await _dbContext.JobPostings
            .FullyPopulatedJobPostingQuery(p => p.Id == postingId)
            .SelectJopPostingResponseFromFullJobPosting(editorProfile.Id)
            .FirstAsync();

        return updatedPostingEntity;
    }

    public async Task<bool> DeleteJobPostingAsync(string postingId, Profile deleterProfile)
    {
        ArgumentException.ThrowIfNullOrEmpty(postingId);
        ArgumentNullException.ThrowIfNull(deleterProfile);

        var jobPosting = await _dbContext.JobPostings.FirstOrDefaultAsync(p => p.Id == postingId);

        if (jobPosting == null)
        {
            return true; // Idempotency: "already deleted" is a success for DELETE
        }

        if (jobPosting.AuthorId != deleterProfile.Id)
        {
            // Add role checks if moderators/admins can delete
            _logger.LogWarning(
                "User {DeleterProfileId} attempted to delete job posting {JobPostingId} owned by {AuthorId}.",
                deleterProfile.Id, jobPosting.Id, jobPosting.AuthorId);

            throw new ForbiddenAccessException(
                $"User {deleterProfile.Id} is not authorized to delete post {postingId}.");
        }

        _dbContext.JobPostings
            .Remove(jobPosting); // EF handles cascading for PostMedia, PostVotes, Comments if configured
        await _dbContext.SaveChangesAsync();

        // Optional: Send to a pipeline if deletion needs further processing (e.g., remove from search index)
        // await _postDeletionChannel.Writer.WriteAsync(new PostDeletedMessage { PostId = postId });

        return true;
    }

    /// <summary>
    ///     Processes upvote/downvote on a job posting with toggle mechanics and tag inheritance.
    /// </summary>
    /// <param name="postingId">The job posting ID to vote on.</param>
    /// <param name="voterProfile">The user casting the vote.</param>
    /// <param name="isUpvoteIntent">True for upvote, false for downvote.</param>
    /// <returns>Updated JobPostingResponse with new vote counts.</returns>
    /// <exception cref="JobPostingNotFoundException">If posting not found.</exception>
    /// <remarks>
    ///     Toggle voting: same vote removes it, opposite vote flips it. Voting adds job's tags to voter's
    ///     UserInterests (skill discovery). Sends notification to author on upvote. Cannot vote on own posting.
    /// </remarks>
    public async Task<JobPostingResponse> VoteJobPostingAsync(
        string postingId,
        Profile voterProfile,
        bool isUpvoteIntent)
    {
        ArgumentException.ThrowIfNullOrEmpty(postingId);
        ArgumentNullException.ThrowIfNull(voterProfile);

        var jobPosting = await _dbContext.JobPostings
            .Include(p => p.Author) // For notification to post author
            .Include(p => p.JobPostingTags) // Include tags for tagging service
            .ThenInclude(pt => pt.Tag)
            .FirstOrDefaultAsync(p => p.Id == postingId);

        if (jobPosting == null)
        {
            throw new PostNotFoundException($"JobPosting with id={postingId} was not found for voting.");
        }

        var vote = await _dbContext.JobPostingVotes
            //.Include(v => v.Post)
            //.ThenInclude(p => p.Author) // Needed for notification message creation. One hit to DB <3
            .FirstOrDefaultAsync(v => v.JobPostingId == postingId && v.ProfileId == voterProfile.Id);

        if (vote == null)
        {
            vote = new JobPostingVote
            {
                ProfileId = voterProfile.Id,
                Profile = voterProfile,
                JobPostingId = jobPosting.Id,
                JobPosting = jobPosting,
                IsUpvote = isUpvoteIntent
            };

            await _dbContext.JobPostingVotes.AddAsync(vote);
            await _taggingService.AddTagsToUserProfileAsync(
                voterProfile.Id,
                jobPosting.JobPostingTags.Select(pt => pt.Tag)
            );
        }
        else
        {
            if (vote.IsUpvote == isUpvoteIntent) // Voting same way again (e.g. upvoting already upvoted)
            {
                _dbContext.JobPostingVotes.Remove(vote);
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
            vote.JobPosting =
                jobPosting; // Just to make sure whatever conditional path we take, the rich post is always attached to the vote for notifications purposes.
            await _notificationFacade.NotifyJobPostingVotedAsync(vote);
        }

        // No second SaveChanges needed if NotifyPostVotedAsync only writes to channel

        var updatedJobPosting = await _dbContext.JobPostings
            .FullyPopulatedJobPostingQuery(p => p.Id == postingId)
            .SelectJopPostingResponseFromFullJobPosting(voterProfile.Id)
            .FirstAsync();

        return updatedJobPosting;
    }

    public async Task<JobApplicationResponse> ApplyToJobPostingAsync(
        string postingId,
        Profile applicantProfile,
        ApplyToJobPostingRequest request)
    {
        ArgumentException.ThrowIfNullOrEmpty(postingId);
        ArgumentNullException.ThrowIfNull(applicantProfile);
        ArgumentNullException.ThrowIfNull(request);

        var jobPosting = await _dbContext.JobPostings
            .Include(p => p.Author) // For notification to post author
            .Include(p => p.JobPostingTags)
            .ThenInclude(pt => pt.Tag) // Include tags for tagging service
            .FirstOrDefaultAsync(p => p.Id == postingId);

        if (jobPosting == null)
        {
            throw new PostNotFoundException($"JobPosting with id={postingId} was not found for application.");
        }

        var existingApplication = await _dbContext.JobApplications
            .AnyAsync(a => a.JobPostingId == postingId && a.ApplicantId == applicantProfile.Id);

        if (existingApplication)
        {
            throw new BadHttpRequestException($"You have already applied to this job posting {postingId}.");
        }

        var jobApplication = new JobApplication
        {
            JobPostingId = postingId,
            ApplicantId = applicantProfile.Id,
            Applicant = applicantProfile,
            CoverLetter = request.CoverLetter?.Trim(),
            OfferedCost = request.OfferedCost
        };

        await _dbContext.JobApplications.AddAsync(jobApplication);
        await _dbContext.SaveChangesAsync();

        await _taggingService.AddTagsToUserProfileAsync(
            applicantProfile.Id,
            jobPosting.JobPostingTags.Select(pt => pt.Tag)
        );

        // Notify the job posting author about the new application
        await _notificationFacade.NotifyJobApplicationSubmittedAsync(jobApplication);

        return jobApplication.SelectJobApplicationResponseFromEntity();
    }

    /// <summary>
    ///     Retrieves all applications for a job posting (authorization: job author only).
    /// </summary>
    /// <param name="jobPostingId">The job posting ID.</param>
    /// <param name="userProfile">The requesting user profile.</param>
    /// <returns>List of applications with applicant details.</returns>
    /// <exception cref="JobPostingNotFoundException">If job posting not found.</exception>
    /// <exception cref="UnauthorizedException">If requester is not the job author.</exception>
    /// <remarks>
    ///     Only job author can view applications. Returns list ordered by application creation date.
    ///     Includes applicant profile, cover letter, expected budget, application status.
    /// </remarks>
    public async Task<List<JobApplicationResponse>> GetJobApplicationsAsync(string jobPostingId, Profile userProfile)
    {
        ArgumentException.ThrowIfNullOrEmpty(jobPostingId);
        ArgumentNullException.ThrowIfNull(userProfile);

        var jobPosting = await _dbContext.JobPostings
            .Include(p => p.JobApplications)
            .ThenInclude(a => a.Applicant)
            .ThenInclude(p => p.Comments) // for the reputation calculation
            .FirstOrDefaultAsync(p => p.Id == jobPostingId);

        if (jobPosting == null)
        {
            throw new PostNotFoundException(
                $"JobPosting with id={jobPostingId} was not found for fetching applications.");
        }

        if (jobPosting.AuthorId != userProfile.Id)
        {
            _logger.LogWarning(
                "User {UserProfileId} attempted to access job applications for posting {JobPostingId} owned by {AuthorId}.",
                userProfile.Id, jobPosting.Id, jobPosting.AuthorId);

            throw new UnauthorizedException(
                $"User {userProfile.Id} is not authorized to view applications for post {jobPostingId}.");
        }

        return jobPosting.JobApplications
            .Select(a => a.SelectJobApplicationResponseFromEntity())
            .ToList();
    }
}
