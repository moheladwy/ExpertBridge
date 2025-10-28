using ExpertBridge.Application.Services;
using ExpertBridge.Application.Settings;
using ExpertBridge.Contract.Messages;
using ExpertBridge.Core.Entities;
using ExpertBridge.Core.Entities.ModerationReports;
using ExpertBridge.Core.Exceptions;
using ExpertBridge.Data.DatabaseContexts;
using ExpertBridge.Notifications;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ExpertBridge.Worker.Consumers;

/// <summary>
///     Consumer for detecting inappropriate comments using the NsfwContentDetectionService.
/// </summary>
/// <remarks>
///     This class processes messages to analyze comment content, generate moderation reports, and handle comment removal
///     and notifications when content violates community guidelines.
/// </remarks>
public sealed class InappropriateCommentDetectionConsumer : IConsumer<DetectInappropriateCommentMessage>
{
    /// <summary>
    ///     An instance of <see cref="ExpertBridgeDbContext" /> used to interact with the application's database for performing
    ///     CRUD operations on comments and moderation reports.
    /// </summary>
    /// <remarks>
    ///     This context facilitates the storage and retrieval of data as part of the inappropriate comment detection process.
    /// </remarks>
    private readonly ExpertBridgeDbContext _dbContext;

    /// <summary>
    ///     An instance of <see cref="NsfwContentDetectionService" /> responsible for detecting
    ///     inappropriate language in comments using AI-powered content moderation.
    /// </summary>
    private readonly NsfwContentDetectionService _detectionService;

    /// <summary>
    ///     An instance of <see cref="ILogger{TCategoryName}" /> used for logging events, errors, and debugging information
    ///     during the inappropriate comment detection and moderation process.
    /// </summary>
    private readonly ILogger<InappropriateCommentDetectionConsumer> _logger;

    /// <summary>
    ///     An instance of <see cref="NotificationFacade" /> used to send notifications
    ///     about moderation actions, such as comment deletions due to policy violations.
    /// </summary>
    private readonly NotificationFacade _notifications;

    /// <summary>
    ///     Configured thresholds for detecting inappropriate language categories such as toxicity,
    ///     obscenity, insults, threats, and identity attacks.
    /// </summary>
    /// <remarks>
    ///     These thresholds determine whether a comment exceeds permissible limits for specific
    ///     categories of inappropriate content during the moderation process.
    /// </remarks>
    private readonly InappropriateLanguageThresholds _thresholds;

    /// <summary>
    ///     Initializes a new instance of the <see cref="InappropriateCommentDetectionConsumer" /> class.
    /// </summary>
    /// <param name="logger">Logger for logging actions and errors.</param>
    /// <param name="detectionService">Service for detecting inappropriate language in comments.</param>
    /// <param name="thresholds">Thresholds for inappropriate language detection.</param>
    /// <param name="dbContext">Database context for accessing comments and moderation reports.</param>
    /// <param name="notifications">Notification facade for sending notifications.</param>
    /// <exception cref="ArgumentNullException">Thrown if any dependency is null.</exception>
    public InappropriateCommentDetectionConsumer(
        ILogger<InappropriateCommentDetectionConsumer> logger,
        NsfwContentDetectionService detectionService,
        IOptionsSnapshot<InappropriateLanguageThresholds> thresholds,
        ExpertBridgeDbContext dbContext,
        NotificationFacade notifications)
    {
        _logger = logger;
        _detectionService = detectionService;
        _thresholds = thresholds.Value;
        _dbContext = dbContext;
        _notifications = notifications;
    }

    /// <summary>
    ///     Consumes a <see cref="DetectInappropriateCommentMessage" /> to detect inappropriate content in a comment.
    /// </summary>
    /// <param name="context">The consume context containing the message.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method analyzes the comment content, generates a moderation report, removes inappropriate comments, and sends
    ///     notifications.
    /// </remarks>
    /// <exception cref="RemoteServiceCallFailedException">Thrown if the detection service returns null.</exception>
    public async Task Consume(ConsumeContext<DetectInappropriateCommentMessage> context)
    {
        try
        {
            var message = context.Message;
            _logger.LogDebug("Detecting inappropriate language for CommentId: {CommentId}", message.CommentId);
            var results = await _detectionService.DetectAsync(message.Content);

            if (results is null)
            {
                _logger.LogError("Detection service returned null for CommentId: {CommentId}", message.CommentId);
                throw new RemoteServiceCallFailedException(
                    $"Error: Inappropriate language service returned null for comment={message.CommentId}.");
            }

            var existingComment = await _dbContext.Comments
                .FirstOrDefaultAsync(c => c.Id == message.CommentId, context.CancellationToken);

            if (existingComment == null)
            {
                _logger.LogWarning("Comment with Id {CommentId} not found in database.", message.CommentId);
                return;
            }

            var isAppropriate = true;
            var reason = "No issues.";

            // Log detection results
            _logger.LogDebug("Detection results for CommentId {CommentId}: {@Results}", message.CommentId, results);

            if (results.Insult >= _thresholds.Insult
                || results.SexualExplicit >= _thresholds.SexualExplicit
                || results.Toxicity >= _thresholds.Toxicity
                || results.SevereToxicity >= _thresholds.SevereToxicity
                || results.Threat >= _thresholds.Threat
                || results.IdentityAttack >= _thresholds.IdentityAttack
                || results.Obscene >= _thresholds.Obscene)
            {
                isAppropriate = false;
                reason = "Your comment does not follow our Community Guidelines.";
                _logger.LogInformation("CommentId {CommentId} flagged as inappropriate.", message.CommentId);
            }
            else
            {
                _logger.LogInformation("CommentId {CommentId} passed moderation.", message.CommentId);
            }

            var report = new ModerationReport
            {
                ContentType = ContentTypes.Comment,
                AuthorId = existingComment.AuthorId,
                ContentId = existingComment.Id,
                IsNegative = !isAppropriate,
                Reason = reason,
                IsResolved =
                    true, // Because this is an automated report generation, not issued by a user of the application
                IdentityAttack = results.IdentityAttack,
                Obscene = results.Obscene,
                Insult = results.Insult,
                SevereToxicity = results.SevereToxicity,
                SexualExplicit = results.SexualExplicit,
                Threat = results.Threat,
                Toxicity = results.Toxicity
            };

            await _dbContext.ModerationReports.AddAsync(report, context.CancellationToken);
            _logger.LogInformation("Moderation report created for CommentId: {CommentId}", message.CommentId);

            existingComment.IsProcessed = true;
            _logger.LogDebug("CommentId {CommentId} marked as processed.", message.CommentId);

            if (!isAppropriate)
            {
                _dbContext.Comments.Remove(existingComment);
                _logger.LogInformation("CommentId {CommentId} removed from database.", message.CommentId);

                await _notifications.NotifyCommentDeletedAsync(existingComment, report);
                _logger.LogInformation("Notification sent for deleted CommentId: {CommentId}", message.CommentId);
            }

            await _dbContext.SaveChangesAsync(context.CancellationToken);
            _logger.LogInformation("Database changes saved for CommentId: {CommentId}", message.CommentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing InappropriateCommentDetectionConsumer for CommentId: {CommentId}",
                context.Message.CommentId);
        }
    }
}
