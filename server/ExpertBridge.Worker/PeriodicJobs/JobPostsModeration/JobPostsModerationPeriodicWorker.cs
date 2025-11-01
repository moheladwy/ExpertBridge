// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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
    ///     Database context instance for accessing and managing JobPosts and related entities within the system.
    ///     Provides a connection to the database and enables querying and updating JobPosts data.
    /// </summary>
    private readonly ExpertBridgeDbContext _dbContext;

    /// <summary>
    ///     Logger instance for logging job execution and errors.
    /// </summary>
    private readonly ILogger<JobPostsModerationPeriodicWorker> _logger;

    /// <summary>
    ///     Endpoint for publishing messages related to job post moderation to the message broker.
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

    /// <summary>
    ///     Executes the job postings moderation job.
    /// </summary>
    /// <param name="context">The job execution context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method processes job postings through three stages:
    ///     1. Safety checking for inappropriate content
    ///     2. Tagging with AI-generated tags
    ///     3. Embedding generation for candidate matching
    /// </remarks>
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            _logger.LogInformation("Starting JobPosts moderation periodic job.");

            var jobPostsToBeSafeChecked = await _dbContext.JobPostings
                .AsNoTracking()
                .Where(p => !p.IsProcessed)
                .Select(p => new DetectInappropriatePostMessage
                {
                    PostId = p.Id,
                    AuthorId = p.AuthorId,
                    Content = p.Content,
                    Title = p.Title,
                    IsJobPosting = true
                })
                .ToListAsync(context.CancellationToken);

            _logger.LogInformation("Found {Count} JobPosts to be safe-checked.", jobPostsToBeSafeChecked.Count);
            foreach (var posts in jobPostsToBeSafeChecked)
            {
                await _publishEndpoint.Publish(posts, context.CancellationToken);
            }

            var jobPostsToBeTagged = await _dbContext.JobPostings
                .AsNoTracking()
                .Where(p => p.IsProcessed && !p.IsTagged)
                .Select(p => new TagPostMessage
                {
                    PostId = p.Id,
                    AuthorId = p.AuthorId,
                    Content = p.Content,
                    Title = p.Title,
                    IsJobPosting = true
                })
                .ToListAsync(context.CancellationToken);

            _logger.LogInformation("Found {Count} JobPosts to be tagged.", jobPostsToBeTagged.Count);
            foreach (var posts in jobPostsToBeTagged)
            {
                await _publishEndpoint.Publish(posts, context.CancellationToken);
            }

            var jobPostsToBeEmbedded = await _dbContext.JobPostings
                .AsNoTracking()
                .Where(p => p.IsProcessed && p.Embedding == null)
                .Select(p => new EmbedPostMessage
                {
                    PostId = p.Id, Content = p.Content, Title = p.Title, IsJobPosting = true
                })
                .ToListAsync(context.CancellationToken);

            _logger.LogInformation("Found {Count} JobPosts to be embedded.", jobPostsToBeEmbedded.Count);
            foreach (var posts in jobPostsToBeEmbedded)
            {
                await _publishEndpoint.Publish(posts, context.CancellationToken);
            }

            await _dbContext.JobPostings
                .Where(jp => jp.IsProcessed && !jp.IsSafeContent && jp.IsTagged && jp.Embedding != null)
                .ExecuteUpdateAsync(set =>
                        set.SetProperty(jobPosting => jobPosting.IsSafeContent, true),
                    context.CancellationToken);

            _logger.LogInformation("JobPosts moderation periodic job completed.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred during JobPosts moderation periodic job execution.");
        }
    }
}
