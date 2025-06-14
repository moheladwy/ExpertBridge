// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Threading.Channels;
using ExpertBridge.Core.Entities.Media.PostMedia;
using ExpertBridge.Core.Entities.Posts;
using ExpertBridge.Core.Entities.PostVotes;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Exceptions;
using ExpertBridge.Core.Queries; // Assuming PostQueries exist here
using ExpertBridge.Core.Requests.CreatePost;
using ExpertBridge.Core.Requests.EditPost;
using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using ExpertBridge.Api.Models.IPC; // For PostProcessingPipelineMessage
using ExpertBridge.Notifications; // For NotificationFacade
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ExpertBridge.Core.Requests;
using ExpertBridge.Api.DomainServices;
using Pgvector.EntityFrameworkCore;
using Pgvector;
using ExpertBridge.Core.EntityConfiguration;
using System.Text; // For logging

namespace ExpertBridge.Api.Services
{
    public class PostService
    {
        private readonly ExpertBridgeDbContext _dbContext;
        private readonly UserService _userService; // Or your specific UserService implementation
        private readonly MediaAttachmentService _mediaService;
        private readonly NotificationFacade _notificationFacade;
        private readonly Channel<PostProcessingPipelineMessage> _postProcessingChannel;
        private readonly ILogger<PostService> _logger;

        public PostService(
            ExpertBridgeDbContext dbContext,
            UserService userService, // Or your UserService
            MediaAttachmentService mediaService,
            NotificationFacade notificationFacade,
            Channel<PostProcessingPipelineMessage> postProcessingChannel,
            ILogger<PostService> logger)
        {
            _dbContext = dbContext;
            _userService = userService;
            _mediaService = mediaService;
            _notificationFacade = notificationFacade;
            _postProcessingChannel = postProcessingChannel;
            _logger = logger;
        }

        public async Task<PostResponse> CreatePostAsync(CreatePostRequest request, Profile authorProfile)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(authorProfile);
            // Your controller already checks for empty title/content, but service can re-validate
            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Content))
            {
                throw new BadHttpRequestException("Title and Content are required."); // Or a more specific ArgumentException
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
                PostMedia createPostMediaFunc(MediaObjectRequest mediaReq, Post parentPost) => new PostMedia
                {
                    Post = parentPost,
                    Name = SanitizeMediaName(parentPost.Title), // Helper from CommentService or shared
                    Type = mediaReq.Type,
                    Key = mediaReq.Key,
                };

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
            await _postProcessingChannel.Writer.WriteAsync(new PostProcessingPipelineMessage
            {
                AuthorId = post.AuthorId, // Use post.AuthorId from the saved entity
                Content = post.Content,
                PostId = post.Id,       // Use post.Id from the saved entity
                Title = post.Title,
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

        private string SanitizeMediaName(string contentHint, int maxLength = 50) // Shared helper
        {
            if (string.IsNullOrWhiteSpace(contentHint)) return "UntitledMedia";
            var name = contentHint.Trim();
            if (name.Length > maxLength)
            {
                name = name.Substring(0, maxLength);
            }
            return name;
        }

        // Implement other IPostService methods...
        public async Task<PostResponse?> GetPostByIdAsync(string postId, string? currentMaybeUserProfileId)
        {
            ArgumentException.ThrowIfNullOrEmpty(postId);

            var postEntity = await _dbContext.Posts
                .FullyPopulatedPostQuery(p => p.Id == postId)
                .SelectPostResponseFromFullPost(currentMaybeUserProfileId) // This does the mapping
                .FirstOrDefaultAsync();

            return postEntity; // Null if not found, controller handles 404
        }

        public async Task<List<PostResponse>> GetAllPostsAsync(string? currentMaybeUserProfileId)
        {
            return await _dbContext.Posts
                .FullyPopulatedPostQuery()
                .SelectPostResponseFromFullPost(currentMaybeUserProfileId)
                .ToListAsync();
        }

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
                var rand = new Random();
                var sb = new StringBuilder();
                sb.Append("0.1");

                for (int i = 0; i < 1023; i++)
                {
                    double value = rand.NextDouble(); // gives value between 0.0 and 1.0
                    sb.Append($",{value.ToString("0.#")}"); // optional: format to reduce decimal noise
                }

                userEmbedding = new Vector(sb.ToString());
            }

            // 2. Build the query for posts
            var query = _dbContext.Posts
                .AsNoTracking()
                .AsQueryable();

            // Apply cursor pagination predicate if not the first page
            if (request.After.HasValue && request.LastIdCursor != null)
            {
                // Assuming lower distance is better (more similar)
                // We want posts with:
                // - distance < lastDistance (more similar)
                // OR
                // - distance == lastDistance AND Id > lastPostId (same similarity, break tie with Id, assuming newer posts have higher IDs or some consistent order)
                // Adjust p.Id > or < lastPostIdCursor.Value based on your tie-breaking sort preference (e.g., if you also sort by CreatedAt DESC)
                query = query.Where(p =>
                    (p.Embedding.CosineDistance(userEmbedding) < request.After.Value) ||
                    (
                        p.Embedding.CosineDistance(userEmbedding) == request.After.Value
                        //&& p.Id.CompareTo(request.LastIdCursor) // Tie-breaker: use Post ID. Adjust if secondary sort is different (e.g. newest first)
                    )
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
                .OrderBy(x => x.Distance)      // Order by similarity (ascending distance)
                .ThenBy(x => x.PostId)        // Consistent tie-breaker
                .Take(request.PageSize + 1)            // Fetch one extra item to determine if there's a next page
                .ToListAsync(cancellationToken);

            // 3. Determine if there's a next page and prepare the results
            bool hasNextPage = postsWithDistance.Count > request.PageSize;
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
                    EndCursor = nextDistance,
                    NextIdCursor = nextPostId,
                    HasNextPage = hasNextPage
                }
            };
        }

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
                .SelectPostResponseFromFullPost(currentMaybeUserProfileId) // Or profileId if that's the vote perspective desired
                .ToListAsync();
        }

        public async Task<PostResponse> EditPostAsync(string postId, EditPostRequest request, Profile editorProfile)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentException.ThrowIfNullOrEmpty(postId);
            ArgumentNullException.ThrowIfNull(editorProfile);

            var post = await _dbContext.Posts
                .FirstOrDefaultAsync(p => p.Id == postId);

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

            bool changed = false;
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
                await _dbContext.SaveChangesAsync();

                // Send to post processing pipeline if content changed
                if (!string.IsNullOrWhiteSpace(request.Content)) // Only if content changed, title change might not need this pipeline
                {
                    await _postProcessingChannel.Writer.WriteAsync(new PostProcessingPipelineMessage
                    {
                        AuthorId = post.AuthorId,
                        Content = post.Content,
                        PostId = post.Id,
                        Title = post.Title, // Send current title
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

        public async Task<PostResponse> VotePostAsync(string postId, Profile voterProfile, bool isUpvoteIntent)
        {
            ArgumentException.ThrowIfNullOrEmpty(postId);
            ArgumentNullException.ThrowIfNull(voterProfile);

            var post = await _dbContext.Posts
                .Include(p => p.Author) // For notification to post author
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
                    IsUpvote = isUpvoteIntent,
                };

                await _dbContext.PostVotes.AddAsync(vote);
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
}


