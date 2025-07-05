// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Threading.Channels;
using Amazon.Runtime.Internal.Util;
using ExpertBridge.Api.DataGenerator;
using ExpertBridge.Api.Models.IPC;
using ExpertBridge.Core.Entities.JobApplications;
using ExpertBridge.Core.Entities.JobPostings;
using ExpertBridge.Core.Entities.JobPostingsVotes;
using ExpertBridge.Core.Entities.Media.JobPostingMedia;
using ExpertBridge.Core.Entities.Media.PostMedia;
using ExpertBridge.Core.Entities.PostVotes;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Exceptions;
using ExpertBridge.Core.Queries;
using ExpertBridge.Core.Requests;
using ExpertBridge.Core.Requests.EditPost;
using ExpertBridge.Core.Requests.JobPostings;
using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using ExpertBridge.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace ExpertBridge.Api.DomainServices
{
    public class JobPostingService
    {
        private readonly ExpertBridgeDbContext _dbContext;
        private readonly MediaAttachmentService _mediaService;
        private readonly TaggingService _taggingService;
        private readonly NotificationFacade _notificationFacade;
        private readonly HybridCache _cache;
        private readonly ChannelWriter<PostProcessingPipelineMessage> _postProcessingChannel;
        private readonly ILogger<JobPostingService> _logger;

        public JobPostingService(
            ExpertBridgeDbContext dbContext,
            MediaAttachmentService mediaService,
            TaggingService taggingService,
            NotificationFacade notificationFacade,
            Channel<PostProcessingPipelineMessage> postProcessingChannel,
            HybridCache cache,
            ILogger<JobPostingService> logger)
        {
            _dbContext = dbContext;
            _mediaService = mediaService;
            _taggingService = taggingService;
            _notificationFacade = notificationFacade;
            _cache = cache;
            _postProcessingChannel = postProcessingChannel.Writer;
            _logger = logger;
        }

        public async Task<JobPostingResponse> CreateAsync(
            CreateJobPostingRequest request,
            Profile authorProfile)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(authorProfile);
            // Your controller already checks for empty title/content, but service can re-validate
            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Content))
            {
                throw new BadHttpRequestException("Title and Content are required."); // Or a more specific ArgumentException
            }

            var posting = new JobPosting
            {
                Title = request.Title.Trim(),
                Area = request.Area.Trim(),
                Budget = request.Budget,
                Content = request.Content.Trim(),
                AuthorId = authorProfile.Id,
                Author = authorProfile, // For navigation, if needed before save by other logic
            };

            await _dbContext.JobPostings.AddAsync(posting);

            if (request.Media?.Count > 0)
            {
                JobPostingMedia createPostMediaFunc(MediaObjectRequest mediaReq, JobPosting parentPost)
                    => new JobPostingMedia
                    {
                        JobPosting = parentPost,
                        Name = _mediaService.SanitizeMediaName(parentPost.Title),
                        Type = mediaReq.Type,
                        Key = mediaReq.Key,
                    };

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
            await _postProcessingChannel.WriteAsync(new PostProcessingPipelineMessage
            {
                AuthorId = posting.AuthorId, // Use post.AuthorId from the saved entity
                Content = posting.Content,
                PostId = posting.Id,       // Use post.Id from the saved entity
                Title = posting.Title,
                IsJobPosting = true,
            });

            return posting.SelectJopPostingResponseFromFullJobPosting(authorProfile.Id);
        }

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
                    Post = p.SelectJopPostingResponseFromFullJobPosting(userProfileId), // The whole post entity for now, will project to DTO later
                    Distance = p.Embedding.CosineDistance(userEmbedding)
                })
                .OrderBy(x => x.Distance)      // Order by similarity (ascending distance)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize + 1)            // Fetch one extra item to determine if there's a next page
                .ToListAsync(cancellationToken);

            // 3. Determine if there's a next page and prepare the results
            bool hasNextPage = postingsWithDistance.Count > request.PageSize;

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
                    HasNextPage = hasNextPage,
                    Embedding = randomEmbedding ?? request.Embedding
                }
            };
        }

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
                async (entry) =>
                {
                    var currentPostEmbeddings = await _dbContext.JobPostings
                        .Where(p => p.Id == postingId && p.Embedding != null)
                        .Select(p => p.Embedding)
                        .FirstOrDefaultAsync(entry);

                    if (currentPostEmbeddings == null)
                    {
                        _logger.LogWarning("No embeddings found for job posting {PostingId} when fetching similar posts.", postingId);
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

        public async Task<List<SimilarJobsResponse>> GetSuggestedJobsAsync(
                Profile? userProfile,
                int limit,
                CancellationToken cancellationToken = default)
        {
            var cacheKey = $"SuggestedJobs_{userProfile?.Id ?? "Anonymous"}_{limit}";

            var similarJobs = await _cache.GetOrCreateAsync<List<SimilarJobsResponse>>(
                cacheKey,
                async (entry) =>
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
                _logger.LogWarning("User {EditorProfileId} attempted to edit job posting {JobPostingId} owned by {AuthorId}.",
                    editorProfile.Id, jobPosting.Id, jobPosting.AuthorId);

                throw new ForbiddenAccessException($"User {editorProfile.Id} is not authorized to edit job posting {postingId}.");
            }

            bool changed = false;
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
                if (!string.IsNullOrWhiteSpace(request.Content)) // Only if content changed, title change might not need this pipeline
                {
                    await _postProcessingChannel.WriteAsync(new PostProcessingPipelineMessage
                    {
                        AuthorId = jobPosting.AuthorId,
                        Content = jobPosting.Content,
                        PostId = jobPosting.Id,
                        Title = jobPosting.Title, // Send current title
                        IsJobPosting = true,
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
                _logger.LogWarning("User {DeleterProfileId} attempted to delete job posting {JobPostingId} owned by {AuthorId}.",
                    deleterProfile.Id, jobPosting.Id, jobPosting.AuthorId);

                throw new ForbiddenAccessException($"User {deleterProfile.Id} is not authorized to delete post {postingId}.");
            }

            _dbContext.JobPostings.Remove(jobPosting); // EF handles cascading for PostMedia, PostVotes, Comments if configured
            await _dbContext.SaveChangesAsync();

            // Optional: Send to a pipeline if deletion needs further processing (e.g., remove from search index)
            // await _postDeletionChannel.Writer.WriteAsync(new PostDeletedMessage { PostId = postId });

            return true;
        }

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
                    IsUpvote = isUpvoteIntent,
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
                vote.JobPosting = jobPosting; // Just to make sure whatever conditional path we take, the rich post is always attached to the vote for notifications purposes.
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
                OfferedCost = request.OfferedCost,
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

                throw new UnauthorizedException($"User {userProfile.Id} is not authorized to view applications for post {jobPostingId}.");
            }

            return jobPosting.JobApplications
                .Select(a => a.SelectJobApplicationResponseFromEntity())
                .ToList();
        }
    }
}
