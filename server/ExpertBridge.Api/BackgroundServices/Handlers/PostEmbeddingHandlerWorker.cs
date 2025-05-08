// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.


using System.Threading.Channels;
using ExpertBridge.Core;
using ExpertBridge.Data.DatabaseContexts;
using ExpertBridge.Api.EmbeddingService;
using ExpertBridge.Api.Models.IPC;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ExpertBridge.Core.Exceptions;

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
                var existingPost = await dbContext.Posts
                    .FirstOrDefaultAsync(p => p.Id == post.PostId, stoppingToken);

                if (existingPost is not null)
                {
                    existingPost.Embedding = embedding;
                    await dbContext.SaveChangesAsync(stoppingToken);
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
