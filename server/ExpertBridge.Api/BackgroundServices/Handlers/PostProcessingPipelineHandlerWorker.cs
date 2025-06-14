// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.


using System.Threading.Channels;
using ExpertBridge.Api.Models.IPC;
using Serilog;

namespace ExpertBridge.Api.BackgroundServices.Handlers
{
    /// <summary>
    /// This worker is responsible for handling the post-processing pipeline.
    /// From start (Inappropriate detection) to end (Embedding).
    /// </summary>
    public class PostProcessingPipelineHandlerWorker
        : HandlerWorker<PostProcessingPipelineHandlerWorker, PostProcessingPipelineMessage>
    {
        private readonly ILogger<PostProcessingPipelineHandlerWorker> _logger;
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
            : base(nameof(PostProcessingPipelineHandlerWorker), postProcessingChannel.Reader, logger)
        {
            _logger = logger;
            _detectionChannel = detectionChannel.Writer;
            _tagPostChannel = tagPostChannel.Writer;
            _embedPostChannel = embedPostChannel.Writer;
            _acknowledgeChannel = acknowledgeChannel.Reader;
        }

        protected override async Task ExecuteInternalAsync(
            PostProcessingPipelineMessage post,
            CancellationToken stoppingToken)
        {
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
                    return;
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
}
