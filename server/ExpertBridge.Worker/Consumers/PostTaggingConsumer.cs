// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Application.DomainServices;
using ExpertBridge.Application.Services;
using ExpertBridge.Core.Messages;
using MassTransit;

namespace ExpertBridge.Worker.Consumers;

/// <summary>
///     Consumer class responsible for processing incoming messages containing information
///     about posts and their associated details, and performing post-tagging operations.
/// </summary>
/// <remarks>
///     This consumer handles messages of type <see cref="TagPostMessage" />. It uses the
///     <see cref="AiPostTaggingService" /> to generate tags for the post based on its content and title.
///     The generated tags are then added to the post using the <see cref="TaggingService" />.
///     Errors that occur during processing are logged with the relevant details.
/// </remarks>
public sealed class PostTaggingConsumer : IConsumer<TagPostMessage>
{
    /// <summary>
    ///     Instance of <see cref="AiPostTaggingService" /> used for generating tags for posts based on their content and
    ///     title.
    /// </summary>
    /// <remarks>
    ///     This service is a core dependency for the consumer class <see cref="PostTaggingConsumer" />.
    ///     It processes text data to automatically generate relevant tags for posts using an underlying
    ///     GroqLlmProvider and ensures resilience through specified pipelines.
    /// </remarks>
    private readonly AiPostTaggingService _aiPostTaggingService;

    /// <summary>
    ///     Logger instance for capturing and recording information, warnings, and errors
    ///     encountered during the execution of the <see cref="PostTaggingConsumer" />.
    /// </summary>
    /// <remarks>
    ///     This logger is used to log diagnostic information and handle exceptions that occur
    ///     while processing messages of type <see cref="TagPostMessage" />. It ensures that any
    ///     issues during the post-tagging process are properly documented for troubleshooting.
    /// </remarks>
    private readonly ILogger<PostTaggingConsumer> _logger;

    /// <summary>
    ///     Represents an instance of <see cref="TaggingService" /> used to manage and associate tags with posts
    ///     during post-tagging operations.
    /// </summary>
    /// <remarks>
    ///     This service is utilized within the <see cref="PostTaggingConsumer" /> to persist tags generated
    ///     for posts. It provides methods for adding raw tags to posts, ensuring that post data is updated
    ///     with relevant tagging details as part of the post processing workflow.
    /// </remarks>
    private readonly TaggingService _taggingService;

    /// <summary>
    ///     Consumes messages related to tagging posts and processes them.
    /// </summary>
    /// <remarks>
    ///     This class is responsible for handling incoming tag-related messages and processing them
    ///     using the provided services. It operates as part of a messaging or event-driven architecture.
    /// </remarks>
    public PostTaggingConsumer(
        ILogger<PostTaggingConsumer> logger,
        AiPostTaggingService aiPostTaggingService,
        TaggingService taggingService)
    {
        _logger = logger;
        _aiPostTaggingService = aiPostTaggingService;
        _taggingService = taggingService;
    }

    public async Task Consume(ConsumeContext<TagPostMessage> context)
    {
        try
        {
            var post = context.Message;
            var tags = await _aiPostTaggingService
                .GeneratePostTagsAsync(post.Title, post.Content, new List<string>());

            if (tags is null)
            {
                _logger.LogWarning("No tags were generated for post with id={PostId}.", context.Message.PostId);
                return;
            }

            // Atomic Operation.
            await _taggingService.AddRawTagsToPostAsync(
                post.PostId,
                post.AuthorId,
                post.IsJobPosting,
                tags,
                context.CancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while processing post with id={context.Message.PostId}.");
        }
    }
}
