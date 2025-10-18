// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Messages;
using ExpertBridge.Data.DatabaseContexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace ExpertBridge.Worker.PeriodicJobs.PostModeration;

/// <summary>
///     Periodic worker job responsible for moderating Posts in the system.
///     This job scans unprocessed posts and publishes messages to the appropriate processing pipelines.
/// </summary>
/// <remarks>
///     The worker ensures that requests to external moderation services (such as GROQ API)
///     are rate-limited by queuing messages for downstream processing.
/// </remarks>
internal sealed class PostsModerationPeriodicWorker : IJob
{
    /// <summary>
    ///     Database context instance for accessing and managing JobPosts and related entities within the system.
    ///     Provides a connection to the database and enables querying and updating JobPosts data.
    /// </summary>
    private readonly ExpertBridgeDbContext _dbContext;

    /// <summary>
    ///     Logger instance for logging job execution and errors.
    /// </summary>
    private readonly ILogger<PostsModerationPeriodicWorker> _logger;

    /// <summary>
    ///     Endpoint for publishing messages related to job post moderation to the message broker.
    /// </summary>
    private readonly IPublishEndpoint _publishEndpoint;

    /// <summary>
    ///     Initializes a new instance of the <see cref="PostsModerationPeriodicWorker" /> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="dbContext"></param>
    /// <param name="publishEndpoint"></param>
    public PostsModerationPeriodicWorker(
        ILogger<PostsModerationPeriodicWorker> logger,
        ExpertBridgeDbContext dbContext,
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            _logger.LogInformation("Starting Posts moderation periodic job.");

            var postsToBeSafeChecked = await _dbContext.Posts
                .AsNoTracking()
                .Where(p => !p.IsProcessed)
                .Select(p => new DetectInappropriatePostMessage
                {
                    PostId = p.Id,
                    AuthorId = p.AuthorId,
                    Content = p.Content,
                    Title = p.Title,
                    IsJobPosting = false
                })
                .ToListAsync(context.CancellationToken);

            _logger.LogInformation("Found {Count} Posts to be safe-checked.", postsToBeSafeChecked.Count);
            foreach (var post in postsToBeSafeChecked)
            {
                await _publishEndpoint.Publish(post, context.CancellationToken);
            }

            var postsToBeTagged = await _dbContext.Posts
                .AsNoTracking()
                .Where(p => p.IsProcessed && !p.IsTagged)
                .Select(p => new TagPostMessage
                {
                    PostId = p.Id,
                    AuthorId = p.AuthorId,
                    Content = p.Content,
                    Title = p.Title,
                    IsJobPosting = false
                })
                .ToListAsync(context.CancellationToken);

            _logger.LogInformation("Found {Count} Posts to be tagged.", postsToBeTagged.Count);
            foreach (var post in postsToBeTagged)
            {
                await _publishEndpoint.Publish(post, context.CancellationToken);
            }

            var postsToBeEmbedded = await _dbContext.Posts
                .AsNoTracking()
                .Where(p => p.IsProcessed && p.Embedding == null)
                .Select(p => new EmbedPostMessage
                {
                    PostId = p.Id, Content = p.Content, Title = p.Title, IsJobPosting = false
                })
                .ToListAsync(context.CancellationToken);

            _logger.LogInformation("Found {Count} Posts to be embedded.", postsToBeEmbedded.Count);
            foreach (var post in postsToBeEmbedded)
            {
                await _publishEndpoint.Publish(post, context.CancellationToken);
            }

            _logger.LogInformation("Published all Posts moderation messages.");

            _logger.LogInformation("Updating Posts and Job Postings with inappropriate content...");
            await _dbContext.Posts
                .Where(p => p.IsProcessed && p.IsTagged && p.Embedding != null && !p.IsSafeContent)
                .ExecuteUpdateAsync(setPropertyCalls =>
                        setPropertyCalls.SetProperty(post => post.IsSafeContent, true),
                    context.CancellationToken);

            _logger.LogInformation("Posts moderation periodic job completed.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred during Posts moderation periodic job execution.");
        }
    }
}
