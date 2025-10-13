// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Contract.Messages;
using ExpertBridge.Data.DatabaseContexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace ExpertBridge.Worker.PeriodicJobs.CommentsModeration;

internal sealed class CommentsModerationPeriodicWorker : IJob
{
    /// <summary>
    ///     Database context for accessing posts, job postings, and comments.
    /// </summary>
    private readonly ExpertBridgeDbContext _dbContext;

    /// <summary>
    ///     Logger instance for logging job execution and errors.
    /// </summary>
    private readonly ILogger<CommentsModerationPeriodicWorker> _logger;

    /// <summary>
    ///     Endpoint for publishing moderation and processing messages.
    /// </summary>
    private readonly IPublishEndpoint _publishEndpoint;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CommentsModerationPeriodicWorker" /> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="dbContext">The database context.</param>
    /// <param name="publishEndpoint">The message bus publishes an endpoint.</param>
    public CommentsModerationPeriodicWorker(
        ILogger<CommentsModerationPeriodicWorker> logger,
        ExpertBridgeDbContext dbContext,
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await _dbContext.Comments
            .AsNoTracking()
            .Where(c => !c.IsProcessed && !c.IsSafeContent)
            .Select(c => new DetectInappropriateCommentMessage
            {
                CommentId = c.Id, AuthorId = c.AuthorId, Content = c.Content
            })
            .ForEachAsync(async void (detectInappropriateCommentMessage) =>
                {
                    try
                    {
                        await _publishEndpoint
                            .Publish(detectInappropriateCommentMessage, context.CancellationToken);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Failed to send comment to inappropriate comment channel.");
                    }
                },
                context.CancellationToken
            );

        await _dbContext.Comments
            .Where(c => !c.IsProcessed && c.IsSafeContent)
            .ExecuteUpdateAsync(setPropertyCalls =>
                    setPropertyCalls.SetProperty(comment => comment.IsProcessed, true),
                context.CancellationToken);
    }
}
