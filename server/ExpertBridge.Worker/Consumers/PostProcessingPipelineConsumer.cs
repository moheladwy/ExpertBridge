// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Messages;
using MassTransit;

namespace ExpertBridge.Worker.Consumers;

public sealed class PostProcessingPipelineConsumer : IConsumer<PostProcessingPipelineMessage>
{
    private readonly ILogger<PostProcessingPipelineConsumer> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public PostProcessingPipelineConsumer(
        ILogger<PostProcessingPipelineConsumer> logger,
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<PostProcessingPipelineMessage> context)
    {
        try
        {
            var post = context.Message;
            _logger.LogDebug("Processing PostId: {PostId}", post.PostId);

            _logger.LogDebug("Publishing DetectInappropriatePostMessage for PostId: {PostId}", post.PostId);
            await _publishEndpoint.Publish(
                new DetectInappropriatePostMessage
                {
                    IsJobPosting = post.IsJobPosting,
                    PostId = post.PostId,
                    Title = post.Title,
                    Content = post.Content,
                    AuthorId = post.AuthorId
                }, context.CancellationToken);

            _logger.LogDebug("Finished processing PostId: {PostId}", post.PostId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error processing PostId: {PostId}", context.Message.PostId);
        }
    }
}
