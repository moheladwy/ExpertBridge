// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Contract.Messages;
using MassTransit;

namespace ExpertBridge.Worker.Consumers;

/// <summary>
///     Consumer that orchestrates the post-processing pipeline by publishing messages
///     for inappropriate content detection.
/// </summary>
/// <remarks>
///     This consumer receives post processing requests and publishes them to the
///     inappropriate content detection queue for moderation.
/// </remarks>
public sealed class PostProcessingPipelineConsumer : IConsumer<PostProcessingPipelineMessage>
{
    /// <summary>
    ///     Logger instance for logging processing events and errors.
    /// </summary>
    private readonly ILogger<PostProcessingPipelineConsumer> _logger;

    /// <summary>
    ///     Endpoint for publishing messages to the message broker.
    /// </summary>
    private readonly IPublishEndpoint _publishEndpoint;

    /// <summary>
    ///     Initializes a new instance of the <see cref="PostProcessingPipelineConsumer" /> class.
    /// </summary>
    /// <param name="logger">The logger instance for diagnostic logging.</param>
    /// <param name="publishEndpoint">The publish endpoint for sending messages to the broker.</param>
    public PostProcessingPipelineConsumer(
        ILogger<PostProcessingPipelineConsumer> logger,
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    /// <summary>
    ///     Consumes a post processing pipeline message and publishes it for inappropriate content detection.
    /// </summary>
    /// <param name="context">The consume context containing the post processing message.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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
