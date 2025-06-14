// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.


using System.Threading.Channels;
using ExpertBridge.Api.Models;
using ExpertBridge.Api.Models.IPC;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ExpertBridge.Api.BackgroundServices.PeriodicJobs
{
    public class PostTaggingPeriodicWorker : PeriodicWorker<PostTaggingPeriodicWorker>
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<PostTaggingPeriodicWorker> _logger;
        private readonly ChannelWriter<TagPostMessage> _postCreatedChannel;

        public PostTaggingPeriodicWorker(
            IServiceProvider services,
            ILogger<PostTaggingPeriodicWorker> logger,
            Channel<TagPostMessage> postCreatedChannel)
            : base(
                PeriodicJobsStartDelays.PostTaggingPeriodicWorkerStartDelay,
                nameof(PostTaggingPeriodicWorker),
                logger)
        {
            _services = services;
            _logger = logger;
            _postCreatedChannel = postCreatedChannel.Writer;
        }

        protected override async Task ExecuteInternalAsync(CancellationToken stoppingToken)
        {
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
        }
    }
}
