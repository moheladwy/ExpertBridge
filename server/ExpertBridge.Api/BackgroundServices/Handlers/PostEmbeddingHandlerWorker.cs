// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.


using System.Threading.Channels;
using ExpertBridge.Api.EmbeddingService;
using ExpertBridge.Api.Models.IPC;
using ExpertBridge.Core.Entities;
using ExpertBridge.Core.Exceptions;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ExpertBridge.Api.BackgroundServices.Handlers
{
    public class PostEmbeddingHandlerWorker : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ChannelReader<EmbedPostMessage> _channel;
        private readonly ILogger<PostEmbeddingHandlerWorker> _logger;
        public PostEmbeddingHandlerWorker(
            IServiceProvider services,
            Channel<EmbedPostMessage> channel,
            ILogger<PostEmbeddingHandlerWorker> logger)
        {
            _services = services;
            _channel = channel.Reader;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (await _channel.WaitToReadAsync(stoppingToken))
                {
                    var post = await _channel.ReadAsync(stoppingToken);

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
            catch (Exception ex)
            {
                // _logger.LogError(ex,
                //     @$"{nameof(PostEmbeddingHandlerWorker)} ran into unexpected error:
                //     An error occurred while reading from the channel.");
                Log.Error(ex,
                    "An error occurred while reading from the channel in {nameof(PostEmbeddingHandlerWorker)}.",
                    nameof(PostEmbeddingHandlerWorker));
            }
            finally
            {
                // _logger.LogInformation($"Terminating {nameof(PostEmbeddingHandlerWorker)}.");
                Log.Information("Terminating {nameof(PostEmbeddingHandlerWorker)}.", nameof(PostEmbeddingHandlerWorker));
            }
        }
    }
}
