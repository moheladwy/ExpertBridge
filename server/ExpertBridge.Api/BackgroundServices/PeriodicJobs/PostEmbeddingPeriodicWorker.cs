using System.Threading.Channels;
using ExpertBridge.Application.Models;
using ExpertBridge.Application.Models.IPC;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ExpertBridge.Api.BackgroundServices.PeriodicJobs
{
    public class PostEmbeddingPeriodicWorker : PeriodicWorker<PostEmbeddingPeriodicWorker>
    {
        private readonly IServiceProvider _services;
        private readonly ChannelWriter<EmbedPostMessage> _embedPostChannel;

        public PostEmbeddingPeriodicWorker(
            IServiceProvider services,
            ILogger<PostEmbeddingPeriodicWorker> logger,
            Channel<EmbedPostMessage> postEmbeddingChannel)
            : base(
                PeriodicJobsStartDelays.PostEmbeddingPeriodicWorkerStartDelay,
                nameof(PostEmbeddingPeriodicWorker),
                logger)
        {
            _services = services;
            _embedPostChannel = postEmbeddingChannel.Writer;
        }

        protected override async Task ExecuteInternalAsync(CancellationToken stoppingToken)
        {
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
                    .Where(p => p.Embedding == null && p.IsProcessed)
                    .Select(p => new EmbedPostMessage
                    {
                        PostId = p.Id,
                        Content = p.Content,
                        Title = p.Title,
                        IsJobPosting = false,
                    })
                    .ForEachAsync(async post =>
                        await _embedPostChannel.WriteAsync(post, stoppingToken),
                        stoppingToken
                    );

                await dbContext.JobPostings
                    .AsNoTracking()
                    .Where(p => p.Embedding == null && p.IsProcessed)
                    .Select(p => new EmbedPostMessage
                    {
                        PostId = p.Id,
                        Content = p.Content,
                        Title = p.Title,
                        IsJobPosting = true,
                    })
                    .ForEachAsync(async post =>
                        await _embedPostChannel.WriteAsync(post, stoppingToken),
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
                // _logger.LogError(ex,
                //     $"Failed to execute {nameof(PostEmbeddingPeriodicWorker)} with exception message {ex.Message}."
                //     );
                Log.Error(ex,
                    "Failed to execute {WorkerName} with exception message {Message}.",
                    nameof(PostEmbeddingPeriodicWorker), ex.Message);
            }
        }
    }
}
