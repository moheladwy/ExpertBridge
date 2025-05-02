// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using System.Threading.Channels;
using Amazon.S3.Model;
using Amazon.S3;
using ExpertBridge.Api.Models.IPC;
using ExpertBridge.Api.Settings;
using ExpertBridge.Core.Entities.Media;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Api.BackgroundServices.PeriodicJobs
{
    public class PostTaggingPeriodicWorker : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<PostTaggingPeriodicWorker> _logger;
        private readonly ChannelWriter<PostCreatedMessage> _postCreatedChannel;

        public PostTaggingPeriodicWorker(
            IServiceProvider services,
            ILogger<PostTaggingPeriodicWorker> logger,
            Channel<PostCreatedMessage> postCreatedChannel)
        {
            _services = services;
            _logger = logger;
            _postCreatedChannel = postCreatedChannel.Writer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // This delay to break the synchronization with the start of each Priodic Worker's period.
            await Task.Delay(TimeSpan.FromHours(8), stoppingToken);

            var period = 60 * 60 * 24 * 1; // 1 day
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(period));

            while (!stoppingToken.IsCancellationRequested
                    && await timer.WaitForNextTickAsync(stoppingToken))
            {
                _logger.LogInformation($"{nameof(PostTaggingPeriodicWorker)} Started...");

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
                        .Where(p => p.IsDeleted == false && !p.IsTagged)
                        .Select(p => new PostCreatedMessage
                        {
                            PostId = p.Id,
                            AuthorId = p.AuthorId,
                            Content = p.Content,
                            Title = p.Title
                        })
                        .ForEachAsync(async post =>
                            await _postCreatedChannel.WriteAsync(post, stoppingToken),
                            stoppingToken
                        );

                    //foreach (var post in unTaggedPosts)
                    //{
                    //    await _postCreatedChannel.WriteAsync(new PostCreatedMessage
                    //    {
                    //        AuthorId = post.AuthorId,
                    //        Content = post.Content,
                    //        PostId = post.Id,
                    //        Title = post.Title
                    //    }, stoppingToken);
                    //}
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        $"Failed to execute {nameof(PostTaggingPeriodicWorker)} with exception message {ex.Message}."
                        );
                }

                _logger.LogInformation($"{nameof(PostTaggingPeriodicWorker)} Finished.");
            }
        }
    }
}
