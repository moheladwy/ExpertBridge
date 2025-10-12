using System.Threading.Channels;
using ExpertBridge.Application.EmbeddingService;
using ExpertBridge.Contract.Messages;
using ExpertBridge.Core.Entities;
using ExpertBridge.Core.Entities.JobPostings;
using ExpertBridge.Core.Exceptions;
using ExpertBridge.Core.Interfaces;
using ExpertBridge.Data.DatabaseContexts;
using ExpertBridge.Notifications;
using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;
using Serilog;

namespace ExpertBridge.Api.BackgroundServices.Handlers
{
    public class PostEmbeddingHandlerWorker
        : HandlerWorker<PostEmbeddingHandlerWorker, EmbedPostMessage>
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<PostEmbeddingHandlerWorker> _logger;
        public PostEmbeddingHandlerWorker(
            IServiceProvider services,
            Channel<EmbedPostMessage> channel,
            ILogger<PostEmbeddingHandlerWorker> logger)
            : base(nameof(PostEmbeddingHandlerWorker), channel.Reader, logger)
        {
            _services = services;
            _logger = logger;
        }
        protected override async Task ExecuteInternalAsync(
            EmbedPostMessage post,
            CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _services.CreateScope();
                var embeddingService = scope.ServiceProvider.GetRequiredService<IEmbeddingService>();
                var embedding = await embeddingService.GenerateEmbedding($"{post.Title} {post.Content}");

                if (embedding is null)
                {
                    throw new RemoteServiceCallFailedException(
                        $"Error: Embedding service returned null embedding for post=${post.PostId}.");
                }

                var dbContext = scope.ServiceProvider.GetRequiredService<ExpertBridgeDbContext>();

                IRecommendableContent? existingPost = null;

                if (post.IsJobPosting)
                {
                    existingPost = await dbContext.JobPostings
                        .FirstOrDefaultAsync(p => p.Id == post.PostId, stoppingToken);
                }
                else
                {
                    existingPost = await dbContext.Posts
                        .FirstOrDefaultAsync(p => p.Id == post.PostId, stoppingToken);
                }

                if (existingPost is null)
                {
                    return;
                }

                existingPost.Embedding = embedding;
                await dbContext.SaveChangesAsync(stoppingToken);

                if (post.IsJobPosting)
                {
                    // CONSIDER! Not the most effecient query due to the duplicate calculation of CosineDistance.
                    // But should not be that heavy on the database considering the relatively small number of users
                    // compared to the number of posts.

                    var candidates = await dbContext.Profiles
                        .Where(p => p.UserInterestEmbedding != null && embedding.CosineDistance(p.UserInterestEmbedding) < 1.0)
                        .OrderBy(p => embedding.CosineDistance(p.UserInterestEmbedding))
                        .ToListAsync(stoppingToken);

                    var notifications = scope.ServiceProvider.GetRequiredService<NotificationFacade>();
                    await notifications.NotifyJobMatchAsync((JobPosting)existingPost!, candidates);
                }
            }
            catch (Exception ex)
            {
                // I commented out the logger because it was not being used in serilog.
                // _logger.LogError(ex, $"An error occurred while processing " +
                //     $"message with post id={post.PostId}.");
                Log.Error(ex,
                    "An error occurred while processing message with post id={post.PostId}.",
                    post.PostId);
            }

        }
    }
}
