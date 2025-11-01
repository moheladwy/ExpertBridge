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

    /// <summary>
    ///     Executes the comments moderation job.
    /// </summary>
    /// <param name="context">The job execution context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method publishes unprocessed comments for inappropriate content detection
    ///     and marks processed comments as safe content.
    /// </remarks>
    public async Task Execute(IJobExecutionContext context)
    {
        var comments = await _dbContext.Comments
            .AsNoTracking()
            .Where(c => !c.IsProcessed)
            .Select(c => new DetectInappropriateCommentMessage
            {
                CommentId = c.Id, AuthorId = c.AuthorId, Content = c.Content
            })
            .ToListAsync(context.CancellationToken);

        _logger.LogInformation("Found {Count} Comments to be safe-checked.", comments.Count);
        foreach (var comment in comments)
        {
            await _publishEndpoint.Publish(comment, context.CancellationToken);
        }

        await _dbContext.Comments
            .Where(c => c.IsProcessed)
            .ExecuteUpdateAsync(setPropertyCalls =>
                    setPropertyCalls.SetProperty(comment => comment.IsSafeContent, true),
                context.CancellationToken);
    }
}
