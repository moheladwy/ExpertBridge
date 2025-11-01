// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Application.Services;
using ExpertBridge.Application.Settings;
using ExpertBridge.Contract.Messages;
using ExpertBridge.Core.Entities;
using ExpertBridge.Core.Entities.ModerationReports;
using ExpertBridge.Core.Exceptions;
using ExpertBridge.Core.Interfaces;
using ExpertBridge.Data.DatabaseContexts;
using ExpertBridge.Notifications;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ExpertBridge.Worker.Consumers;

public sealed class InappropriatePostDetectionConsumer : IConsumer<DetectInappropriatePostMessage>
{
    /// <summary>
    ///     Database context for reading/updating posts and writing moderation reports.
    /// </summary>
    private readonly ExpertBridgeDbContext _dbContext;

    /// <summary>
    ///     Service that performs inappropriate language detection using the Groq provider.
    /// </summary>
    private readonly NsfwContentDetectionService _detectionService;

    /// <summary>
    ///     Logger for emitting information, warning and error messages during the consume flow.
    /// </summary>
    private readonly ILogger<InappropriatePostDetectionConsumer> _logger;

    /// <summary>
    ///     Notification facade used to send notifications when posts are removed.
    /// </summary>
    private readonly NotificationFacade _notifications;

    /// <summary>
    ///     The endpoint for publishing messages to the message broker.
    /// </summary>
    /// <remarks>
    ///     This is used to publish notification events after inappropriate content detection processing.
    /// </remarks>
    private readonly IPublishEndpoint _publishEndpoint;

    /// <summary>
    ///     Thresholds used to determine when detection scores are considered violations.
    /// </summary>
    private readonly InappropriateLanguageThresholds _thresholds;

    /// <summary>
    ///     Initializes a new instance of the <see cref="InappropriatePostDetectionConsumer" /> class.
    /// </summary>
    /// <param name="logger">The logger instance to log consumer actions and decisions.</param>
    /// <param name="detectionService">The Groq inappropriate language detection service.</param>
    /// <param name="dbContext">The database context used to lookup and modify posts and moderation reports.</param>
    /// <param name="thresholds">
    ///     Snapshot of <see cref="InappropriateLanguageThresholds" /> used to decide when to mark content
    ///     as inappropriate.
    /// </param>
    /// <param name="publishEndpoint">The MassTransit publish endpoint for sending messages.</param>
    /// <param name="notifications">The notification facade used to send deletion/notification messages.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown by DI container or callers if any required dependency is missing
    ///     (dependency injection should prevent this).
    /// </exception>
    public InappropriatePostDetectionConsumer(
        ILogger<InappropriatePostDetectionConsumer> logger,
        NsfwContentDetectionService detectionService,
        ExpertBridgeDbContext dbContext,
        IOptionsSnapshot<InappropriateLanguageThresholds> thresholds,
        NotificationFacade notifications,
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _detectionService = detectionService;
        _dbContext = dbContext;
        _notifications = notifications;
        _thresholds = thresholds.Value;
        _publishEndpoint = publishEndpoint;
    }

    /// <summary>
    ///     Handles incoming <see cref="DetectInappropriatePostMessage" /> messages. Runs inappropriate-content detection,
    ///     constructs and persists a <see cref="ModerationReport" />, removes offending content when thresholds are exceeded,
    ///     and triggers notifications about deletions.
    /// </summary>
    /// <param name="context">
    ///     The MassTransit <see cref="ConsumeContext{T}" /> carrying the
    ///     <see cref="DetectInappropriatePostMessage" />.
    /// </param>
    /// <returns>A <see cref="Task" /> representing the asynchronous consume operation.</returns>
    /// <remarks>
    ///     - The message's <c>Title</c> and <c>Content</c> are concatenated and sent to <see cref="_detectionService" />.
    ///     - If the service returns null, a <see cref="RemoteServiceCallFailedException" /> is thrown to indicate a failure
    ///     calling the remote detection service.
    ///     - If any detection score meets or exceeds the configured thresholds, the content is considered inappropriate, a
    ///     moderation report is created,
    ///     the content is removed from the database, and a notification is dispatched.
    ///     - The method marks the existing post as processed regardless of the result and persists the moderation report and
    ///     changes.
    /// </remarks>
    /// <exception cref="RemoteServiceCallFailedException">
    ///     Thrown when the detection service returns null or indicates a
    ///     failure to produce results.
    /// </exception>
    /// <exception cref="DbUpdateException">
    ///     May be thrown by EF Core when saving changes to the database (e.g.,
    ///     <see cref="ExpertBridgeDbContext.SaveChangesAsync(CancellationToken)" />).
    /// </exception>
    public async Task Consume(ConsumeContext<DetectInappropriatePostMessage> context)
    {
        var post = context.Message;
        _logger.LogInformation("Received message to detect inappropriate content for PostId={PostId}", post.PostId);

        var results = await _detectionService.DetectAsync($"{post.Title} {post.Content}");

        if (results is null)
        {
            _logger.LogError("Inappropriate language service returned null for PostId={PostId}", post.PostId);
            throw new RemoteServiceCallFailedException(
                $"Error: Inappropriate language service returned null for post=${post.PostId}.");
        }

        IRecommendableContent? existingPost;

        if (post.IsJobPosting)
        {
            _logger.LogInformation("Looking up JobPosting with Id={PostId}", post.PostId);
            existingPost = await _dbContext.JobPostings
                .FirstOrDefaultAsync(p => p.Id == post.PostId, context.CancellationToken);
        }
        else
        {
            _logger.LogInformation("Looking up Post with Id={PostId}", post.PostId);
            existingPost = await _dbContext.Posts
                .FirstOrDefaultAsync(p => p.Id == post.PostId, context.CancellationToken);
        }

        if (existingPost is null)
        {
            _logger.LogWarning("No post found with Id={PostId}. Skipping moderation.", post.PostId);
            return;
        }

        var isAppropriate = true;
        var reason = "No issues.";

        if (results.Insult > _thresholds.Insult
            || results.SexualExplicit > _thresholds.SexualExplicit
            || results.Toxicity > _thresholds.Toxicity
            || results.SevereToxicity > _thresholds.SevereToxicity
            || results.Threat > _thresholds.Threat
            || results.IdentityAttack > _thresholds.IdentityAttack
            || results.Obscene > _thresholds.Obscene)
        {
            isAppropriate = false;
            reason = "Your post does not follow our Community Guidelines.";
            _logger.LogWarning("PostId={PostId} flagged as inappropriate. Reason: {Reason}", post.PostId, reason);
        }
        else
        {
            _logger.LogInformation("PostId={PostId} passed moderation checks.", post.PostId);
        }

        var report = new ModerationReport
        {
            ContentType = post.IsJobPosting ? ContentTypes.JobPosting : ContentTypes.Post,
            ReportedBy = ReportedBy.Ai,
            AuthorId = existingPost.AuthorId,
            ContentId = existingPost.Id,
            IsNegative = !isAppropriate,
            Reason = reason,
            IsResolved = false, // Because this is an automated report generation, not issued by a user of the application
            IdentityAttack = results.IdentityAttack,
            Obscene = results.Obscene,
            Insult = results.Insult,
            SevereToxicity = results.SevereToxicity,
            SexualExplicit = results.SexualExplicit,
            Threat = results.Threat,
            Toxicity = results.Toxicity
        };

        await _dbContext.ModerationReports.AddAsync(report, context.CancellationToken);
        _logger.LogInformation("Moderation report created for PostId={PostId}", post.PostId);

        existingPost.IsProcessed = true;

        if (!isAppropriate)
        {
            _dbContext.Remove(existingPost);
            _logger.LogInformation("PostId={PostId} removed from database due to inappropriate content.", post.PostId);

            await _notifications.NotifyPostDeletedAsync(existingPost, report);
            _logger.LogInformation("Notification sent for deleted PostId={PostId}", post.PostId);
        }
        else
        {
            _logger.LogDebug("Publishing EmbedPostMessage for PostId: {PostId}", post.PostId);
            await _publishEndpoint.Publish(
                new TagPostMessage
                {
                    PostId = post.PostId,
                    AuthorId = post.AuthorId,
                    Content = post.Content,
                    Title = post.Title,
                    IsJobPosting = post.IsJobPosting
                }, context.CancellationToken);

            _logger.LogDebug("Publishing EmbedPostMessage for PostId: {PostId}", post.PostId);
            await _publishEndpoint.Publish(
                new EmbedPostMessage
                {
                    PostId = post.PostId,
                    Content = post.Content,
                    Title = post.Title,
                    IsJobPosting = post.IsJobPosting
                }, context.CancellationToken);
        }

        await _dbContext.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Changes saved to database for PostId={PostId}", post.PostId);
    }
}
