// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using System.Threading.Channels;
using Api.Models.IPC;
using Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Api.BackgroundServices.PeriodicJobs
{
    public class PostTaggingPeriodicWorker : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<PostTaggingPeriodicWorker> _logger;
        private readonly ChannelWriter<TagPostMessage> _postCreatedChannel;

        public PostTaggingPeriodicWorker(
            IServiceProvider services,
            ILogger<PostTaggingPeriodicWorker> logger,
            Channel<TagPostMessage> postCreatedChannel)
        {
            _services = services;
            _logger = logger;
            _postCreatedChannel = postCreatedChannel.Writer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);

            var period = 60 * 60 * 24 * 1; // 1 day
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(period));

            while (!stoppingToken.IsCancellationRequested
                    && await timer.WaitForNextTickAsync(stoppingToken))
            {
                // _logger.LogInformation($"{nameof(PeriodicPostTaggingWorker)} Started...");
                Log.Information("{WorkerName} Started...", nameof(PostTaggingPeriodicWorker));

                try
                {
                    using var scope = _services.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ExpertBridgeDbContext>();

                    var unTaggedPosts = await dbContext.Posts
                        .AsNoTracking()
                        .Where(p => p.IsDeleted == false && !p.IsTagged && p.IsProcessed)
                        .Select(p => new
                        {
                            p.Id,
                            p.AuthorId,
                            p.Content,
                            p.Title
                        })
                        .ToListAsync(stoppingToken);

                    // That might look like a weird design decision.
                    // But look, the GROQ API we are using is limited, and thus
                    // we need to make sure that we are not sending too many requests
                    // The queing nature of the PostCreatedWorker allows us to send the requests
                    // one by one ensuring that we are not exceeding the rate limit.

                    foreach (var post in unTaggedPosts)
                    {
                        await _postCreatedChannel.WriteAsync(new TagPostMessage
                        {
                            AuthorId = post.AuthorId,
                            Content = post.Content,
                            PostId = post.Id,
                            Title = post.Title
                        }, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    // _logger.LogError(ex,
                    //     $"Failed to execute {nameof(PeriodicPostTaggingWorker)} with exception message {ex.Message}."
                    //     );
                    Log.Error(ex, "Failed to execute {WorkerName} with exception message {Message}.",
                        nameof(PostTaggingPeriodicWorker), ex.Message);
                }

                // _logger.LogInformation($"{nameof(PeriodicPostTaggingWorker)} Finished.");
                Log.Information("{WorkerName} Finished.", nameof(PostTaggingPeriodicWorker));
            }
        }
    }
}
