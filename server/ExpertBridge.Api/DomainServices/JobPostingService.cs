// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Threading.Channels;
using ExpertBridge.Api.Models.IPC;
using ExpertBridge.Core.Entities.JobPostings;
using ExpertBridge.Core.Entities.Media.JobPostingMedia;
using ExpertBridge.Core.Entities.Media.PostMedia;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Requests;
using ExpertBridge.Core.Requests.JobPostings;
using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using ExpertBridge.Notifications;

namespace ExpertBridge.Api.DomainServices
{
    public class JobPostingService
    {
        private readonly ExpertBridgeDbContext _dbContext;
        private readonly MediaAttachmentService _mediaService;
        private readonly NotificationFacade _notificationFacade;
        private readonly ChannelWriter<PostProcessingPipelineMessage> _postProcessingChannel;
        private readonly ILogger<JobPostingService> _logger;

        public JobPostingService(
            ExpertBridgeDbContext dbContext,
            MediaAttachmentService mediaService,
            NotificationFacade notificationFacade,
            Channel<PostProcessingPipelineMessage> postProcessingChannel,
            ILogger<JobPostingService> logger)
        {
            _dbContext = dbContext;
            _mediaService = mediaService;
            _notificationFacade = notificationFacade;
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

            //return posting.SelectJopPostingResponseFromFullJobPosting(authorProfile.Id);
            return new JobPostingResponse
            {
                Id = posting.Id,
                Title = posting.Title,
                Content = posting.Content,
                CreatedAt = posting.CreatedAt,
                // Add other properties as needed, like Media if you want to return them
            };
        }
    }
}
