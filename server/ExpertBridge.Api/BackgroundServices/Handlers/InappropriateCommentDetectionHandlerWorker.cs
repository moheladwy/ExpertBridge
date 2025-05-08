// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.


using System.Threading.Channels;
using ExpertBridge.Core.Entities;
using ExpertBridge.Core.Entities.ModerationReports;
using ExpertBridge.Data.DatabaseContexts;
using ExpertBridge.Api.Models.IPC;
using ExpertBridge.Api.Services;
using ExpertBridge.Api.Settings;
using ExpertBridge.Core.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;

namespace ExpertBridge.Api.BackgroundServices.Handlers
{
    public class InappropriateCommentDetectionHandlerWorker
        : HandlerWorker<InappropriateCommentDetectionHandlerWorker, DetectInappropriateCommentMessage>
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<InappropriateCommentDetectionHandlerWorker> _logger;

        public InappropriateCommentDetectionHandlerWorker(
            IServiceProvider services,
            Channel<DetectInappropriateCommentMessage> channel,
            ILogger<InappropriateCommentDetectionHandlerWorker> logger)
            : base(nameof(InappropriateCommentDetectionHandlerWorker), channel.Reader, logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteInternalAsync(
            DetectInappropriateCommentMessage comment,
            CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _services.CreateScope();
                var detectionService = scope.ServiceProvider
                    .GetRequiredService<GroqInappropriateLanguageDetectionService>();

                var results = await detectionService.DetectAsync(comment.Content);

                if (results is null)
                {
                    throw new RemoteServiceCallFailedException(
                        $"Error: Inappropriate language service returned null for comment=${comment.CommentId}.");
                }

                var thresholds = scope.ServiceProvider
                    .GetRequiredService<IOptionsSnapshot<InappropriateLanguageThresholds>>()
                    .Value;

                var dbContext = scope.ServiceProvider.GetRequiredService<ExpertBridgeDbContext>();
                var existingComment = await dbContext.Comments
                    .FirstOrDefaultAsync(c => c.Id == comment.CommentId, stoppingToken);

                //var moderationService = scope.ServiceProvider
                //    .GetRequiredService<ContentModerationService>();

                bool isAppropriate = true;
                var reason = "No issues.";

                if (existingComment is not null)
                {
                    if (results.Insult >= thresholds.Insult
                        || results.SexualExplicit >= thresholds.SexualExplicit
                        || results.Toxicity >= thresholds.Toxicity
                        || results.SevereToxicity >= thresholds.SevereToxicity
                        || results.Threat >= thresholds.Threat
                        || results.IdentityAttack >= thresholds.IdentityAttack
                        || results.Obscene >= thresholds.Obscene)
                    {
                        // Mark as inappropriate and mark as deleted ...
                        //await moderationService.ReportPostAsync(post.PostId, results, isNegative: true);
                        isAppropriate = false;
                        reason = "Your comment does not follow our Community Guidelines.";
                    }

                    await dbContext.ModerationReports
                        .AddAsync(new ModerationReport
                        {
                            ContentType = ContentTypes.Comment,
                            AuthorId = existingComment.AuthorId,
                            ContentId = existingComment.Id,
                            IsNegative = !isAppropriate,
                            Reason = reason,
                            IsResolved = true, // Because this is an automated report generation, not issued by a user of the application
                            IdentityAttack = results.IdentityAttack,
                            Obscene = results.Obscene,
                            Insult = results.Insult,
                            SevereToxicity = results.SevereToxicity,
                            SexualExplicit = results.SexualExplicit,
                            Threat = results.Threat,
                            Toxicity = results.Toxicity,
                        }, stoppingToken);

                    existingComment.IsProcessed = true;

                    if (!isAppropriate)
                    {
                        dbContext.Comments.Remove(existingComment);
                    }

                    await dbContext.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, $"An error occurred while processing " +
                //     $"message with comment id={comment.CommentId}.");
                Log.Error(ex, "An error occurred while processing message with comment id={CommentId}.",
                    comment.CommentId);
            }

        }
    }
}
