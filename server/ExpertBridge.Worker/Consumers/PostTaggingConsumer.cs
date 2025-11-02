// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Application.Services;
using ExpertBridge.Contract.Messages;
using MassTransit;

namespace ExpertBridge.Worker.Consumers;

/// <summary>
///     Consumer class responsible for processing incoming messages containing information
///     about posts and performing AI-powered post-tagging operations.
/// </summary>
/// <remarks>
///     This consumer handles messages of type <see cref="TagPostMessage" />. It uses the
///     <see cref="AiPostTaggingService" /> to generate tags for the post based on its content and title.
///     The generated tags are then added to the post using the <see cref="TaggingService" />.
/// </remarks>
public sealed class PostTaggingConsumer : IConsumer<TagPostMessage>
{
    /// <summary>
    ///     Service for generating AI-powered tags for posts based on their content and title.
    /// </summary>
    private readonly AiPostTaggingService _aiPostTaggingService;

    /// <summary>
    ///     Logger instance for capturing information, warnings, and errors
    ///     encountered during post-tagging operations.
    /// </summary>
    private readonly ILogger<PostTaggingConsumer> _logger;

    /// <summary>
    ///     Service for managing and associating tags with posts during tagging operations.
    /// </summary>
    private readonly TaggingService _taggingService;

    /// <summary>
    ///     Initializes a new instance of the <see cref="PostTaggingConsumer" /> class.
    /// </summary>
    /// <param name="logger">The logger instance for diagnostic logging.</param>
    /// <param name="aiPostTaggingService">The AI service for generating post tags.</param>
    /// <param name="taggingService">The service for persisting tags to posts.</param>
    public PostTaggingConsumer(
        ILogger<PostTaggingConsumer> logger,
        AiPostTaggingService aiPostTaggingService,
        TaggingService taggingService)
    {
        _logger = logger;
        _aiPostTaggingService = aiPostTaggingService;
        _taggingService = taggingService;
    }

    /// <summary>
    ///     Consumes a tag post message and generates AI-powered tags for the post.
    /// </summary>
    /// <param name="context">The consume context containing the tag post message.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method generates tags using AI, then atomically adds them to the post in the database.
    ///     If no tags are generated, a warning is logged and processing continues.
    /// </remarks>
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
