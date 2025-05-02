// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Models.IPC;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using System.Threading.Channels;

namespace ExpertBridge.Api.BackgroundServices.PeriodicJobs
{
    public class ContentModerationPeriodicWorker : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ChannelWriter<DetectInappropriatePostMessage> _inappropriatePostChannel;
        private readonly ChannelWriter<DetectInappropriateCommentMessage> _inappropriateCommentChannel;
        private readonly ILogger<ContentModerationPeriodicWorker> _logger;

        public ContentModerationPeriodicWorker(
            IServiceProvider services,
            Channel<DetectInappropriatePostMessage> inappropriatePostChannel,
            Channel<DetectInappropriateCommentMessage> inappropriateCommentChannel,
            ILogger<ContentModerationPeriodicWorker> logger
            )
        {
            _services = services;
            _inappropriatePostChannel = inappropriatePostChannel.Writer;
            _inappropriateCommentChannel = inappropriateCommentChannel.Writer;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // This delay to break the synchronization with the start of each Priodic Worker's period.
            await Task.Delay(TimeSpan.FromHours(4), stoppingToken);

            var period = 60 * 60 * 24 * 1; // 1 day
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(period));

            while (!stoppingToken.IsCancellationRequested
                    && await timer.WaitForNextTickAsync(stoppingToken))
            {
                _logger.LogInformation($"{nameof(ContentModerationPeriodicWorker)} Started...");

                try
                {
                    using var scope = _services.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ExpertBridgeDbContext>();

                    // That might look like a weird design decision.
                    // But look, the GROQ API we are using is limited, and thus
                    // we need to make sure that we are not sending too many requests
                    // The queing nature of the PostCreatedWorker allows us to send the requests
                    // one by one ensuring that we are not exceeding the rate limit.

                    await dbContext.Posts
                        .AsNoTracking()
                        .Where(p => !p.IsProcessed)
                        .Select(p => new DetectInappropriatePostMessage
                        {
                            PostId = p.Id,
                            AuthorId = p.AuthorId,
                            Content = p.Content,
                            Title = p.Title
                        })
                        .ForEachAsync(async post =>
                            await _inappropriatePostChannel.WriteAsync(post, stoppingToken),
                            stoppingToken
                        );

                    await dbContext.Comments
                        .AsNoTracking()
                        .Where(c => !c.IsProcessed)
                        .Select(c => new DetectInappropriateCommentMessage
                        {
                            CommentId = c.Id,
                            AuthorId = c.AuthorId,
                            Content = c.Content,
                        })
                        .ForEachAsync(async comment =>
                            await _inappropriateCommentChannel.WriteAsync(comment, stoppingToken),
                            stoppingToken
                        );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        $"Failed to execute {nameof(ContentModerationPeriodicWorker)} with exception message {ex.Message}."
                        );
                }

                _logger.LogInformation($"{nameof(ContentModerationPeriodicWorker)} Finished.");
            }
        }
    }
}
