// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.


using System.Threading.Channels;
using ExpertBridge.Api.Models.IPC;
using ExpertBridge.Api.Services;
using Serilog;

namespace ExpertBridge.Api.BackgroundServices.Handlers
{
    /// <summary>
    /// This worker is responsible for handling the post-processing pipeline.
    /// From start (Inappropriate detection) to end (Embedding).
    /// </summary>
    public class PostProcessingPipelineHandlerWorker : BackgroundService
    {
        private readonly ILogger<PostProcessingPipelineHandlerWorker> _logger;
        private readonly ChannelReader<PostProcessingPipelineMessage> _postProcessingChannel;
        private readonly ChannelWriter<DetectInappropriatePostMessage> _detectionChannel;
        private readonly ChannelWriter<TagPostMessage> _tagPostChannel;
        private readonly ChannelWriter<EmbedPostMessage> _embedPostChannel;
        private readonly ChannelReader<AcknowledgePostProcessingMessage> _acknowledgeChannel;

        public PostProcessingPipelineHandlerWorker(
            ILogger<PostProcessingPipelineHandlerWorker> logger,
            Channel<PostProcessingPipelineMessage> postProcessingChannel,
            Channel<DetectInappropriatePostMessage> detectionChannel,
            Channel<TagPostMessage> tagPostChannel,
            Channel<EmbedPostMessage> embedPostChannel,
            Channel<AcknowledgePostProcessingMessage> acknowledgeChannel)
        {
            _logger = logger;
            _detectionChannel = detectionChannel.Writer;
            _postProcessingChannel = postProcessingChannel.Reader;
            _tagPostChannel = tagPostChannel.Writer;
            _embedPostChannel = embedPostChannel.Writer;
            _acknowledgeChannel = acknowledgeChannel.Reader;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (await _postProcessingChannel.WaitToReadAsync(stoppingToken))
                {
                    var post = await _postProcessingChannel.ReadAsync(stoppingToken);

                    try
                    {
                        // Here we are providing coordination between inappropriate language
                        // detection, and others collectively. That is, only posts that pass
                        // the inappropriate language detection will be tagged and embedded.

                        await _detectionChannel.WriteAsync(new DetectInappropriatePostMessage
                        {
                            PostId = post.PostId,
                            AuthorId = post.AuthorId,
                            Content = post.Content,
                            Title = post.Title
                        }, stoppingToken);

                        await _acknowledgeChannel.WaitToReadAsync(stoppingToken);
                        var ack = await _acknowledgeChannel.ReadAsync(stoppingToken);

                        if (!ack.IsAppropriate)
                        {
                            continue;
                        }

                        await _tagPostChannel.WriteAsync(new TagPostMessage
                        {
                            PostId = post.PostId,
                            AuthorId = post.AuthorId,
                            Content = post.Content,
                            Title = post.Title
                        }, stoppingToken);

                        await _embedPostChannel.WriteAsync(new EmbedPostMessage
                        {
                            PostId = post.PostId,
                            Content = post.Content,
                            Title = post.Title,
                        }, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        // _logger.LogError(ex, $"An error occurred while processing post with id={post.PostId}.");
                        Log.Error(ex,
                            "Pipeline: An error occurred while processing post with id={post.PostId}.",
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
                    nameof(PostProcessingPipelineHandlerWorker));
            }
            finally
            {
                // _logger.LogInformation($"Terminating {nameof(PostCreatedHandlerWorker)}.");
                Log.Information("Terminating {0}.",
                    nameof(PostProcessingPipelineHandlerWorker));
            }
        }
    }
}
