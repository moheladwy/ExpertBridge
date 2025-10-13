// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Contract.Messages;
using ExpertBridge.Data.DatabaseContexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace ExpertBridge.Worker.PeriodicJobs.JobPostsModeration;

/// <summary>
///     Periodic worker job responsible for moderating JobPosts in the system.
///     This job scans unprocessed JobPosts and publishes messages to the appropriate processing pipelines.
/// </summary>
/// <remarks>
///     The worker ensures that requests to external moderation services (such as GROQ API)
///     are rate-limited by queuing messages for downstream processing.
/// </remarks>
internal sealed class JobPostsModerationPeriodicWorker : IJob
{
    /// <summary>
    ///     Logger instance for logging job execution and errors.
    /// </summary>
    private readonly ILogger<JobPostsModerationPeriodicWorker> _logger;

    /// <summary>
    /// Database context instance for accessing and managing JobPosts and related entities within the system.
    /// Provides a connection to the database and enables querying and updating JobPosts data.
    /// </summary>
    private readonly ExpertBridgeDbContext _dbContext;

    /// <summary>
    /// Endpoint for publishing messages related to job post moderation to the message broker.
    /// </summary>
    private readonly IPublishEndpoint _publishEndpoint;

    /// <summary>
    ///     Initializes a new instance of the <see cref="JobPostsModerationPeriodicWorker" /> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="dbContext">The database context for accessing JobPosts.</param>
    /// <param name="publishEndpoint">The publish endpoint for sending messages.</param>
    public JobPostsModerationPeriodicWorker(
        ILogger<JobPostsModerationPeriodicWorker> logger,
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
            _logger.LogInformation("Starting JobPosts moderation periodic job.");
                    await _dbContext.JobPostings
            .AsNoTracking()
            .Where(p => !p.IsProcessed && !p.IsSafeContent)
            .Select(p => new DetectInappropriatePostMessage
            {
                PostId = p.Id,
                AuthorId = p.AuthorId,
                Content = p.Content,
                Title = p.Title,
                IsJobPosting = true
            })
            .ForEachAsync(async void (detectInappropriatePostMessage) =>
                {
                    try
                    {
                        await _publishEndpoint
                            .Publish(detectInappropriatePostMessage, context.CancellationToken);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Failed to send job posting to post processing pipeline.");
                    }
                },
                context.CancellationToken
            );

        await _dbContext.JobPostings
            .AsNoTracking()
            .Where(p => !p.IsProcessed && p.IsSafeContent && !p.IsTagged)
            .Select(p => new TagPostMessage
            {
                PostId = p.Id,
                AuthorId = p.AuthorId,
                Content = p.Content,
                Title = p.Title,
                IsJobPosting = true
            })
            .ForEachAsync(async void (tagPostMessage) =>
                {
                    try
                    {
                        await _publishEndpoint
                            .Publish(tagPostMessage, context.CancellationToken);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Failed to send job posting to post processing pipeline.");
                    }
                },
                context.CancellationToken
            );

        await _dbContext.JobPostings
            .AsNoTracking()
            .Where(p => p.IsProcessed && p.Embedding == null)
            .Select(p => new EmbedPostMessage
            {
                PostId = p.Id, Content = p.Content, Title = p.Title, IsJobPosting = true
            })
            .ForEachAsync(async void (embedPostMessage) =>
                {
                    try
                    {
                        await _publishEndpoint
                            .Publish(embedPostMessage, context.CancellationToken);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Failed to send job posting to post processing pipeline.");
                    }
                },
                context.CancellationToken
            );

        await _dbContext.JobPostings
            .Where(jp => !jp.IsProcessed && jp.IsSafeContent && jp.IsTagged)
            .ExecuteUpdateAsync(setPropertyCalls =>
                    setPropertyCalls.SetProperty(jobPosting => jobPosting.IsProcessed, true),
                context.CancellationToken);
        _logger.LogInformation("JobPosts moderation periodic job completed.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred during JobPosts moderation periodic job execution.");
        }
    }
}
