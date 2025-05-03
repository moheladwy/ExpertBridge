// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.


using ExpertBridge.Api.Extensions;
using ExpertBridge.Api.Models.IPC;
using ExpertBridge.Api.Services;
using ExpertBridge.Api.Settings;
using ExpertBridge.Core.Entities;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Threading.Channels;
using Serilog;

namespace ExpertBridge.Api.BackgroundServices.Handlers
{
    public class DetectInappropriatePostHandlerWorker : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ChannelReader<DetectInappropriatePostMessage> _channel;
        private readonly ILogger<DetectInappropriatePostHandlerWorker> _logger;

        public DetectInappropriatePostHandlerWorker(
            IServiceProvider services,
            Channel<DetectInappropriatePostMessage> channel,
            ILogger<DetectInappropriatePostHandlerWorker> logger)
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
                        var detectionService = scope.ServiceProvider
                            .GetRequiredService<GroqInappropriateLanguageDetectionService>();

                        var results = await detectionService.DetectAsync($"{post.Title} {post.Content}");

                        if (results is null)
                        {
                            throw new RemoteServiceCallFailedException(
                                $"Error: Inappropriate language service returned null for post=${post.PostId}.");
                        }

                        var thresholds = scope.ServiceProvider
                            .GetRequiredService<IOptionsSnapshot<InappropriateLanguageThresholds>>()
                            .Value;

                        var dbContext = scope.ServiceProvider.GetRequiredService<ExpertBridgeDbContext>();
                        var existingPost = await dbContext.Posts
                            .FirstOrDefaultAsync(p => p.Id == post.PostId, stoppingToken);

                        var moderationService = scope.ServiceProvider
                            .GetRequiredService<ContentModerationService>();

                        if (existingPost is not null)
                        {
                            if (results.Insult >= thresholds.Insult
                                || results.SexualExplicit >= thresholds.SexualExplicit
                                || results.Toxicity >= thresholds.Toxicity
                                || results.SevereToxicity >= thresholds.SevereToxicity
                                || results.Threat >= thresholds.Threat
                                || results.IdentityAttack >= thresholds.IdentityAttack
                                || results.Obscene >= thresholds.Obscene)
                            {
                                // Mark as inappropriate and mark as deleted ...
                                await moderationService.ReportPostAsync(post.PostId, results);
                            }

                            existingPost .IsProcessed = true;
                            await dbContext.SaveChangesAsync(stoppingToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        // _logger.LogError(ex, $"An error occurred while processing " +
                        //     $"message with post id={post.PostId}.");
                        Log.Error(ex,
                            "An error occurred while processing message with post id={PostId}.",
                            post.PostId);
                    }
                }
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex,
                //     @$"{nameof(DetectInappropriatePostHandlerWorker)} ran into unexpected error:
                //     An error occurred while reading from the channel.");
                Log.Error(ex,
                    "{WorkerName} ran into unexpected error: An error occurred while reading from the channel.",
                    nameof(DetectInappropriatePostHandlerWorker));
            }
            finally
            {
                // _logger.LogInformation($"Terminating {nameof(DetectInappropriatePostHandlerWorker)}.");
                Log.Information("Terminating {WorkerName}.", nameof(DetectInappropriatePostHandlerWorker));
            }
        }
    }
}
