// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Amazon.S3;
using Amazon.S3.Model;
using ExpertBridge.Core.Entities.Media;
using ExpertBridge.Data.DatabaseContexts;
using ExpertBridge.Extensions.AWS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;

namespace ExpertBridge.Worker.PeriodicJobs.S3Cleaning;

/// <summary>
///     Represents a periodic worker for cleaning up resources in an AWS S3 bucket.
///     This job interacts with the database, retrieves relevant data, and performs cleanup operations
///     on the S3 bucket based on the defined logic.
/// </summary>
internal sealed class S3CleaningPeriodicWorker : IJob
{
    /// <summary>
    ///     Configuration settings for AWS integrations, including the S3 bucket,
    ///     region, and authentication details.
    /// </summary>
    private readonly AwsSettings _awsSettings;

    /// <summary>
    ///     Database context for accessing posts, job postings, and comments.
    /// </summary>
    private readonly ExpertBridgeDbContext _dbContext;

    /// <summary>
    ///     Logger instance for logging job execution and errors.
    /// </summary>
    private readonly ILogger<S3CleaningPeriodicWorker> _logger;

    /// <summary>
    ///     Instance of Amazon S3 client used for interacting with AWS S3 service,
    ///     including operations like deleting objects for periodic cleaning tasks.
    /// </summary>
    private readonly IAmazonS3 _s3Client;

    /// <summary>
    ///     Initializes a new instance of the <see cref="S3CleaningPeriodicWorker" /> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="dbContext">The database context.</param>
    /// <param name="s3Client">The Amazon S3 client instance.</param>
    /// <param name="awsSettings">The AWS settings.</param>
    public S3CleaningPeriodicWorker(
        ILogger<S3CleaningPeriodicWorker> logger,
        ExpertBridgeDbContext dbContext,
        IAmazonS3 s3Client,
        IOptionsSnapshot<AwsSettings> awsSettings)
    {
        _logger = logger;
        _dbContext = dbContext;
        _s3Client = s3Client;
        _awsSettings = awsSettings.Value;
    }

    /// <summary>
    ///     Executes the S3 bucket cleaning job.
    /// </summary>
    /// <param name="context">The job execution context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method:
    ///     1. Identifies media objects whose parent entities have been deleted
    ///     2. Identifies media grants that have been on hold for more than 1 hour
    ///     3. Removes orphaned objects from S3 storage
    ///     4. Removes corresponding database records
    /// </remarks>
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            var onHoldGrants = await _dbContext.MediaGrants
                .Where(g => g.OnHold && g.GrantedAt < DateTime.UtcNow.AddHours(-1))
                .ToListAsync(context.CancellationToken);

            List<string> validKeys = [];
            List<MediaObject> deletedMedias = [];

            // PostMedias
            await _dbContext.PostMedias
                .IgnoreQueryFilters()
                .Where(m => !m.IsDeleted)
                .Include(m => m.Post)
                .ForEachAsync(media =>
                {
                    if (media.Post is not null && !media.Post.IsDeleted)
                    {
                        validKeys.Add(media.Key);
                    }
                    else
                    {
                        deletedMedias.Add(media);
                    }
                }, context.CancellationToken);

            // CommentMedias
            await _dbContext.CommentMedias
                .IgnoreQueryFilters()
                .Where(m => !m.IsDeleted)
                .Include(m => m.Comment)
                .ForEachAsync(media =>
                {
                    if (media.Comment is not null && !media.Comment.IsDeleted)
                    {
                        validKeys.Add(media.Key);
                    }
                    else
                    {
                        deletedMedias.Add(media);
                    }
                }, context.CancellationToken);

            // ProfileMedias
            await _dbContext.ProfileMedias
                .IgnoreQueryFilters()
                .Where(m => !m.IsDeleted)
                .Include(m => m.Profile)
                .ForEachAsync(media =>
                {
                    if (media.Profile is not null && !media.Profile.IsDeleted)
                    {
                        validKeys.Add(media.Key);
                    }
                    else
                    {
                        deletedMedias.Add(media);
                    }
                }, context.CancellationToken);

            // ChatMedias
            await _dbContext.ChatMedias
                .IgnoreQueryFilters()
                .Where(m => !m.IsDeleted)
                .Include(m => m.Chat)
                .ForEachAsync(media =>
                {
                    if (media.Chat is not null && !media.Chat.IsDeleted)
                    {
                        validKeys.Add(media.Key);
                    }
                    else
                    {
                        deletedMedias.Add(media);
                    }
                }, context.CancellationToken);

            // JobPostingMedias
            await _dbContext.JobPostingMedias
                .IgnoreQueryFilters()
                .Where(m => !m.IsDeleted)
                .Include(m => m.JobPosting)
                .ForEachAsync(media =>
                {
                    if (media.JobPosting is not null && !media.JobPosting.IsDeleted)
                    {
                        validKeys.Add(media.Key);
                    }
                    else
                    {
                        deletedMedias.Add(media);
                    }
                }, context.CancellationToken);

            // ProfileExperienceMedias
            await _dbContext.ProfileExperienceMedias
                .IgnoreQueryFilters()
                .Where(m => !m.IsDeleted)
                .Include(m => m.ProfileExperience)
                .ForEachAsync(media =>
                {
                    if (media.ProfileExperience is not null && !media.ProfileExperience.IsDeleted)
                    {
                        validKeys.Add(media.Key);
                    }
                    else
                    {
                        deletedMedias.Add(media);
                    }
                }, context.CancellationToken);

            if (deletedMedias.Count + onHoldGrants.Count > 0)
            {
                await _s3Client.DeleteObjectsAsync(
                    new DeleteObjectsRequest
                    {
                        BucketName = _awsSettings.BucketName,
                        Objects =
                            deletedMedias.Select(m => m.Key)
                                .Where(k => !validKeys.Contains(k))
                                .Concat(onHoldGrants.Select(g => g.Key))
                                .Select(k => new KeyVersion { Key = k })
                                .ToList()
                    },
                    context.CancellationToken);

                // Delete deleted medias from database
                _dbContext.RemoveRange(deletedMedias);
                _dbContext.RemoveRange(onHoldGrants);
                await _dbContext.SaveChangesAsync(context.CancellationToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e,
                "Error occurred while executing S3 cleaning job, see details below:\n{ExceptionMessage}",
                e.Message);
        }
    }
}
