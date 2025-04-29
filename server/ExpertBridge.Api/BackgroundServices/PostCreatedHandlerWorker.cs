// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using System.Threading.Channels;
using ExpertBridge.Api.HttpClients;
using ExpertBridge.Api.Models;
using ExpertBridge.Api.Requests;

namespace ExpertBridge.Api.BackgroundServices
{
    public class PostCreatedHandlerWorker : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ChannelReader<PostCreatedMessage> _channel;
        private readonly ILogger<PostCreatedHandlerWorker> _logger;

        public PostCreatedHandlerWorker(
            IServiceProvider services,
            Channel<PostCreatedMessage> channel,
            ILogger<PostCreatedHandlerWorker> logger)
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
                        var client = scope.ServiceProvider.GetRequiredService<IPostCategroizerClient>();

                        var response = await client.GetPostTagsAsync(new PostCategorizerRequest
                        {
                            Post = $"{post.Title} {post.Content}",
                        });

                        if (!response.IsSuccessStatusCode)
                        {
                            throw new Exception($"Request to categorizer failed with status code: {response.StatusCode}");
                        }

                        var tags = response.Content;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"An error occurred while processing post with id={post.PostId}.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    $"{nameof(PostCreatedHandlerWorker)} ran into unexpected error: " +
                    $"An error occurred while reading from the channel.");
            }
        }
    }
}
