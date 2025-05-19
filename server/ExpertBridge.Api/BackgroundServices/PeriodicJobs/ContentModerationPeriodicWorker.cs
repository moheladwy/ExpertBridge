// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Threading.Channels;
using ExpertBridge.Api.Models;
using ExpertBridge.Data.DatabaseContexts;
using ExpertBridge.Api.Models.IPC;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ExpertBridge.Api.BackgroundServices.PeriodicJobs
{
    public class ContentModerationPeriodicWorker : PeriodicWorker<ContentModerationPeriodicWorker>
    {
        private readonly IServiceProvider _services;
        private readonly ChannelWriter<PostProcessingPipelineMessage> _postProcessingPipelineChannel;
        private readonly ChannelWriter<DetectInappropriateCommentMessage> _inappropriateCommentChannel;
        private readonly ILogger<ContentModerationPeriodicWorker> _logger;

        public ContentModerationPeriodicWorker(
            IServiceProvider services,
            Channel<PostProcessingPipelineMessage> postProcessingPipelineChannel,
            Channel<DetectInappropriateCommentMessage> inappropriateCommentChannel,
            ILogger<ContentModerationPeriodicWorker> logger)
            : base(
                PeriodicJobsStartDelays.ContentModerationPeriodicWorkerStartDelay,
                nameof(ContentModerationPeriodicWorker),
                logger)
        {
            _services = services;
            _postProcessingPipelineChannel = postProcessingPipelineChannel.Writer;
            _inappropriateCommentChannel = inappropriateCommentChannel.Writer;
            _logger = logger;
        }

        protected override async Task ExecuteInternalAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ExpertBridgeDbContext>();

                // That might look like a weird design decision.
                // But look, the GROQ API we are using is limited, and thus
                // we need to make sure that we are not sending too many requests
                // The queing nature of the PostCreatedWorker allows us to send the requests
                // one by one ensuring that we are not exceeding the rate limit.

                await dbContext.Posts
                    .AsNoTracking()
                    .Where(p => !p.IsProcessed)
                    .Select(p => new PostProcessingPipelineMessage
                    {
                        PostId = p.Id,
                        AuthorId = p.AuthorId,
                        Content = p.Content,
                        Title = p.Title
                    })
                    .ForEachAsync(async post =>
                        await _postProcessingPipelineChannel.WriteAsync(post, stoppingToken),
                        stoppingToken
                    );

                await dbContext.Comments
                    .AsNoTracking()
                    .Where(c => !c.IsProcessed)
                    .Select(c => new DetectInappropriateCommentMessage
                    {
                        CommentId = c.Id,
                        AuthorId = c.AuthorId,
                        Content = c.Content,
                    })
                    .ForEachAsync(async comment =>
                        await _inappropriateCommentChannel.WriteAsync(comment, stoppingToken),
                        stoppingToken
                    );
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex,
                //     $"Failed to execute {nameof(ContentModerationPeriodicWorker)} with exception message {ex.Message}."
                //     );
                Log.Error(ex,
                    "Failed to execute {WorkerName} with exception message {ExceptionMessage}.",
                    nameof(ContentModerationPeriodicWorker), ex.Message);
            }
        }
    }
}
