using ExpertBridge.Contract.Messages;
using ExpertBridge.Data.DatabaseContexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace ExpertBridge.Worker.PeriodicJobs;

/// <summary>
/// Periodic worker job responsible for moderating content in the system.
/// This job scans unprocessed posts, job postings, and comments, and publishes messages to the appropriate processing pipelines.
/// </summary>
/// <remarks>
/// The worker ensures that requests to external moderation services (such as GROQ API) are rate-limited by queuing messages for downstream processing.
/// </remarks>
internal sealed class ContentModerationPeriodicWorker : IJob
{
    /// <summary>
    /// Logger instance for logging job execution and errors.
    /// </summary>
    private readonly ILogger<ContentModerationPeriodicWorker> _logger;

    /// <summary>
    /// Database context for accessing posts, job postings, and comments.
    /// </summary>
    private readonly ExpertBridgeDbContext _dbContext;

    /// <summary>
    /// Endpoint for publishing moderation and processing messages.
    /// </summary>
    private readonly IPublishEndpoint _publishEndpoint;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentModerationPeriodicWorker"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="dbContext">The database context.</param>
    /// <param name="publishEndpoint">The message bus publish endpoint.</param>
    public ContentModerationPeriodicWorker(
        ILogger<ContentModerationPeriodicWorker> logger,
        ExpertBridgeDbContext dbContext,
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }

    /// <summary>
    /// Executes the periodic content moderation job.
    /// Scans for unprocessed posts, job postings, and comments, and publishes them to the appropriate processing pipelines.
    /// </summary>
    /// <param name="context">The job execution context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// Exceptions during publishing individual messages are logged, but do not stop the job from processing other items.
    /// </remarks>
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
                .ForEachAsync(async void (postProcessingPipelineMessage) =>
                    {
                        try
                        {
                            await _publishEndpoint
                                .Publish(postProcessingPipelineMessage, context.CancellationToken);
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
                .ForEachAsync(async void (postProcessingPipelineMessage) =>
                    {
                        try
                        {
                            await _publishEndpoint
                                .Publish(postProcessingPipelineMessage, context.CancellationToken);
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
                .ForEachAsync(async void (detectInappropriateCommentMessage) =>
                    {
                        try
                        {
                            await _publishEndpoint
                                .Publish(detectInappropriateCommentMessage, context.CancellationToken);
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
