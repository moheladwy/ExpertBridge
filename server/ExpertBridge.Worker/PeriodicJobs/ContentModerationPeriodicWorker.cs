using ExpertBridge.Contract.Messages;
using ExpertBridge.Data.DatabaseContexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace ExpertBridge.Worker.PeriodicJobs;

[DisallowConcurrentExecution]
internal sealed class ContentModerationPeriodicWorker : IJob
{
    private readonly ILogger<ContentModerationPeriodicWorker> _logger;
    private readonly ExpertBridgeDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public ContentModerationPeriodicWorker(
        ILogger<ContentModerationPeriodicWorker> logger,
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
            // That might look like a weird design decision.
            // But look, the GROQ API we are using is limited, and thus
            // we need to make sure that we are not sending too many requests
            // The queing nature of the PostCreatedWorker allows us to send the requests
            // one by one ensuring that we are not exceeding the rate limit.
            await _dbContext.Posts
                .AsNoTracking()
                .Where(p => !p.IsProcessed)
                .Select(p => new PostProcessingPipelineMessage
                {
                    PostId = p.Id,
                    AuthorId = p.AuthorId,
                    Content = p.Content,
                    Title = p.Title,
                    IsJobPosting = false,
                })
                .ForEachAsync(async void (post) =>
                    {
                        try
                        {
                            await _publishEndpoint.Publish(post, context.CancellationToken);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, "Failed to send post to post processing pipeline.");
                        }
                    },
                    context.CancellationToken
                );

            await _dbContext.JobPostings
                .AsNoTracking()
                .Where(p => !p.IsProcessed)
                .Select(p => new PostProcessingPipelineMessage
                {
                    PostId = p.Id,
                    AuthorId = p.AuthorId,
                    Content = p.Content,
                    Title = p.Title,
                    IsJobPosting = true,
                })
                .ForEachAsync(async void (post) =>
                    {
                        try
                        {
                            await _publishEndpoint.Publish(post, context.CancellationToken);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, "Failed to send job posting to post processing pipeline.");
                        }
                    },
                    context.CancellationToken
                );

            await _dbContext.Comments
                .AsNoTracking()
                .Where(c => !c.IsProcessed)
                .Select(c => new DetectInappropriateCommentMessage
                {
                    CommentId = c.Id,
                    AuthorId = c.AuthorId,
                    Content = c.Content,
                })
                .ForEachAsync(async void (comment) =>
                    {
                        try
                        {
                            await _publishEndpoint.Publish(comment, context.CancellationToken);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, "Failed to send comment to inappropriate comment channel.");
                        }
                    },
                    context.CancellationToken
                );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                $"Failed to execute {nameof(ContentModerationPeriodicWorker)} with exception message {ex.Message}."
                );
        }
    }
}
