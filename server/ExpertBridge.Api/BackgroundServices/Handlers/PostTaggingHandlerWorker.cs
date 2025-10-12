using System.Threading.Channels;
using ExpertBridge.Application.DomainServices;
using ExpertBridge.Application.Services;
using ExpertBridge.Contract.Messages;
using Serilog;

namespace ExpertBridge.Api.BackgroundServices.Handlers
{
    /// <summary>
    /// Handles the creation of posts and categorizes them using a remote service.
    /// </summary>
    public class PostTaggingHandlerWorker
        : HandlerWorker<PostTaggingHandlerWorker, TagPostMessage>
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<PostTaggingHandlerWorker> _logger;

        public PostTaggingHandlerWorker(
            IServiceProvider services,
            Channel<TagPostMessage> channel,
            ILogger<PostTaggingHandlerWorker> logger)
            : base(nameof(PostTaggingHandlerWorker), channel.Reader, logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteInternalAsync(
            TagPostMessage post,
            CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _services.CreateScope();
                //var client = scope.ServiceProvider.GetRequiredService<IPostCategroizerClient>();

                //var response = await client.GetPostTagsAsync(new PostCategorizerRequest
                //{
                //    Title = post.Title,
                //    Content = post.Content,
                //});

                //if (!response.IsSuccessStatusCode || response.Content == null)
                //{
                //    throw new RemoteServiceCallFailedException(
                //        $"Request to categorizer failed with status code: " +
                //        $"{response.StatusCode}, {response.Error?.Content}");
                //}

                //var tags = response.Content;

                var groqService = scope.ServiceProvider.GetRequiredService<GroqPostTaggingService>();
                var tags = await groqService
                    .GeneratePostTagsAsync(post.Title, post.Content, new List<string>());

                var taggingService = scope.ServiceProvider.GetRequiredService<TaggingService>();

                // Atomic Operation.
                await taggingService.AddRawTagsToPostAsync(post.PostId, post.AuthorId, post.IsJobPosting, tags, stoppingToken);
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, $"An error occurred while processing post with id={post.PostId}.");
                Log.Error(ex,
                    "An error occurred while processing post with id={post.PostId}.",
                    post.PostId);
            }

        }
    }
}
