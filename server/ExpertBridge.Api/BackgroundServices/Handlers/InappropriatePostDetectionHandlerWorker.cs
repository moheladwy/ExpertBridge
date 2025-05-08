// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.


using System.Threading.Channels;
using ExpertBridge.Core;
using ExpertBridge.Core.Entities;
using ExpertBridge.Core.Entities.ModerationReports;
using ExpertBridge.Data.DatabaseContexts;
using ExpertBridge.Api.Models.IPC;
using ExpertBridge.Api.Services;
using ExpertBridge.Api.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using ExpertBridge.Core.Exceptions;
using ExpertBridge.Notifications;

namespace ExpertBridge.Api.BackgroundServices.Handlers
{
    public class InappropriatePostDetectionHandlerWorker
        : HandlerWorker<InappropriatePostDetectionHandlerWorker, DetectInappropriatePostMessage>
    {
        private readonly IServiceProvider _services;
        private readonly ChannelWriter<AcknowledgePostProcessingMessage> _acknowledgeChannel;
        private readonly ILogger<InappropriatePostDetectionHandlerWorker> _logger;

        public InappropriatePostDetectionHandlerWorker(
            IServiceProvider services,
            Channel<DetectInappropriatePostMessage> channel,
            Channel<AcknowledgePostProcessingMessage> acknowledgeChannel,
            ILogger<InappropriatePostDetectionHandlerWorker> logger)
            : base(nameof(InappropriatePostDetectionHandlerWorker), channel.Reader, logger)
        {
            _services = services;
            _acknowledgeChannel = acknowledgeChannel.Writer;
            _logger = logger;
        }

        protected override async Task ExecuteInternalAsync(
            DetectInappropriatePostMessage post,
            CancellationToken stoppingToken)
        {            
            try
            {
                using var scope = _services.CreateScope();
                var detectionService = scope.ServiceProvider
                    .GetRequiredService<GroqInappropriateLanguageDetectionService>();

                var results = await detectionService.DetectAsync($"{post.Title} {post.Content}");

                if (results is null)
                {
                    throw new RemoteServiceCallFailedException(
                        $"Error: Inappropriate language service returned null for post=${post.PostId}.");
                }

                var thresholds = scope.ServiceProvider
                    .GetRequiredService<IOptionsSnapshot<InappropriateLanguageThresholds>>()
                    .Value;

                var dbContext = scope.ServiceProvider.GetRequiredService<ExpertBridgeDbContext>();
                var existingPost = await dbContext.Posts
                    .FirstOrDefaultAsync(p => p.Id == post.PostId, stoppingToken);

                //var moderationService = scope.ServiceProvider
                //    .GetRequiredService<ContentModerationService>();

                bool isAppropriate = true;
                var reason = "No issues.";

                if (existingPost is not null)
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

                    var report = new ModerationReport
                    {
                        ContentType = ContentTypes.Post,
                        AuthorId = existingPost.AuthorId,
                        ContentId = existingPost.Id,
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
                    };

                    await dbContext.ModerationReports.AddAsync(report, stoppingToken);

                    existingPost .IsProcessed = true;

                    if (!isAppropriate)
                    {
                        dbContext.Posts.Remove(existingPost);

                        var notifications = scope.ServiceProvider.GetRequiredService<NotificationFacade>();
                        await notifications.NotifyPostDeletedAsync(existingPost, report);
                    }

                    await dbContext.SaveChangesAsync(stoppingToken);

                    await _acknowledgeChannel.WriteAsync(new AcknowledgePostProcessingMessage
                    {
                        IsAppropriate = isAppropriate,
                    }, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, $"An error occurred while processing " +
                //     $"message with post id={post.PostId}.");
                Log.Error(ex,
                    "An error occurred while processing message with post id={PostId}.",
                    post.PostId);
            }
                
        }
    }
}
