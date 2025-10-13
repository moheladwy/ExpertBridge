// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Application.DomainServices;
using ExpertBridge.Application.Services;
using ExpertBridge.Contract.Messages;
using MassTransit;

namespace ExpertBridge.Worker.Consumers;

internal sealed class PostTaggingConsumer : IConsumer<TagPostMessage>
{
    private readonly GroqPostTaggingService _groqPostTaggingService;
    private readonly ILogger<PostTaggingConsumer> _logger;
    private readonly TaggingService _taggingService;

    public PostTaggingConsumer(
        ILogger<PostTaggingConsumer> logger,
        GroqPostTaggingService groqPostTaggingService,
        TaggingService taggingService)
    {
        _logger = logger;
        _groqPostTaggingService = groqPostTaggingService;
        _taggingService = taggingService;
    }

    public async Task Consume(ConsumeContext<TagPostMessage> context)
    {
        try
        {
            var post = context.Message;
            var tags = await _groqPostTaggingService
                .GeneratePostTagsAsync(post.Title, post.Content, new List<string>());

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
