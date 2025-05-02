// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using System.Threading.Channels;
using ExpertBridge.Api.HttpClients;
using ExpertBridge.Api.Models.IPC;
using ExpertBridge.Api.Requests;
using ExpertBridge.Api.Services;
using ExpertBridge.Core.Entities;
using ExpertBridge.Data.DatabaseContexts;
using Serilog;

namespace ExpertBridge.Api.BackgroundServices.Handlers
{
    /// <summary>
    /// Handles the creation of posts and categorizes them using a remote service.
    /// Responsible for every operation needs to take place when a post is created.
    /// </summary>
    public class PostCreatedHandlerWorker : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ChannelWriter<EmbedPostMessage> _embedPostChannel;
        private readonly ChannelReader<PostCreatedMessage> _postCreatedChannel;
        private readonly ILogger<PostCreatedHandlerWorker> _logger;

        public PostCreatedHandlerWorker(
            IServiceProvider services,
            Channel<PostCreatedMessage> postCreatedChannel,
            Channel<EmbedPostMessage> embedPostChannel,
            ILogger<PostCreatedHandlerWorker> logger)
        {
            _services = services;
            _embedPostChannel = embedPostChannel.Writer;
            _postCreatedChannel = postCreatedChannel.Reader;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (await _postCreatedChannel.WaitToReadAsync(stoppingToken))
                {
                    var post = await _postCreatedChannel.ReadAsync(stoppingToken);

                    try
                    {
                        await _embedPostChannel.WriteAsync(new EmbedPostMessage
                        {
                            PostId = post.PostId,
                            Title = post.Title,
                            Content = post.Content,
                        }, stoppingToken);

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
                        await taggingService.AddRawTagsToPostAsync(post.PostId, post.AuthorId, tags, stoppingToken);
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
            catch (Exception ex)
            {
                // _logger.LogError(ex,
                //     @$"{nameof(PostCreatedHandlerWorker)} ran into unexpected error:
                //     An error occurred while reading from the channel.");
                Log.Error(ex,
                    "An error occurred while reading from the channel in {nameof(PostCreatedHandlerWorker)}.",
                    nameof(PostCreatedHandlerWorker));
            }
            finally
            {
                // _logger.LogInformation($"Terminating {nameof(PostCreatedHandlerWorker)}.");
                Log.Information("Terminating {nameof(PostCreatedHandlerWorker)}.",
                    nameof(PostCreatedHandlerWorker));
            }
        }
    }
}
