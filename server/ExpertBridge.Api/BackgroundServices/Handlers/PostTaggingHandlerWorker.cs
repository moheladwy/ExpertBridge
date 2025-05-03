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
    /// </summary>
    public class PostTaggingHandlerWorker : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ChannelReader<TagPostMessage> _tagPostChannel;
        private readonly ILogger<PostTaggingHandlerWorker> _logger;

        public PostTaggingHandlerWorker(
            IServiceProvider services,
            Channel<TagPostMessage> tagPostChannel,
            ILogger<PostTaggingHandlerWorker> logger)
        {
            _services = services;
            _tagPostChannel = tagPostChannel.Reader;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (await _tagPostChannel.WaitToReadAsync(stoppingToken))
                {
                    var post = await _tagPostChannel.ReadAsync(stoppingToken);

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
                    "An error occurred while reading from the channel in {0}.",
                    nameof(PostTaggingHandlerWorker));
            }
            finally
            {
                // _logger.LogInformation($"Terminating {nameof(PostCreatedHandlerWorker)}.");
                Log.Information("Terminating {0}.",
                    nameof(PostTaggingHandlerWorker));
            }
        }
    }
}
