// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Contract.Messages;
using ExpertBridge.Data.DatabaseContexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace ExpertBridge.Worker.PeriodicJobs.MoveImagesFromGoogleToS3;

/// <summary>
///     A periodic job responsible for migrating profile images from Google Cloud Storage to Amazon S3.
///     This worker retrieves profile data from the database, constructs messages for image migration,
///     and processes these messages through a messaging system to facilitate the migration.
/// </summary>
public sealed class MoveImagesFromGoogleToS3PeriodicWorker : IJob
{
    /// <summary>
    ///     Provides database context access for the <see cref="MoveImagesFromGoogleToS3PeriodicWorker" />.
    ///     This is used to interact with the underlying database, including querying and managing data
    ///     related to entities such as profiles, posts, jobs, and other domain objects.
    /// </summary>
    private readonly ExpertBridgeDbContext _dbContext;

    /// <summary>
    ///     Provides logging capabilities for the <see cref="MoveImagesFromGoogleToS3PeriodicWorker" />.
    ///     This is used to log application events, errors, and other runtime information
    ///     during the execution of the periodic job.
    /// </summary>
    private readonly ILogger<MoveImagesFromGoogleToS3PeriodicWorker> _logger;

    /// <summary>
    ///     Represents the MassTransit endpoint used to publish messages to the messaging system.
    ///     This is utilized within the <see cref="MoveImagesFromGoogleToS3PeriodicWorker" />
    ///     to facilitate event-driven communication and processing.
    /// </summary>
    private readonly IPublishEndpoint _publishEndpoint;

    /// <summary>
    ///     Represents a periodic worker job that handles moving images from Google storage
    ///     to Amazon S3. This job is configured to run at predefined intervals and ensures
    ///     the transfer of image data between the two storage systems.
    /// </summary>
    public MoveImagesFromGoogleToS3PeriodicWorker(
        ILogger<MoveImagesFromGoogleToS3PeriodicWorker> logger,
        ExpertBridgeDbContext dbContext,
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }

    /// <summary>
    ///     Executes the image migration job.
    /// </summary>
    /// <param name="context">The job execution context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method identifies profiles with images hosted on Google Cloud Storage
    ///     and publishes migration messages to move them to Amazon S3.
    /// </remarks>
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            _logger.LogInformation("Starting MoveImagesFromGoogleToS3 periodic job.");

            var messages = await _dbContext.Profiles
                .AsNoTracking()
                .Where(p => p.ProfilePictureUrl != null && p.ProfilePictureUrl.Contains("googleusercontent.com"))
                .Select(p => new MoveProfileImageFromGoogleToS3Message { ProfileId = p.Id })
                .ToListAsync(context.CancellationToken);

            _logger.LogInformation("Found {Count} Profiles to be migrated.", messages.Count);
            foreach (var message in messages)
            {
                await _publishEndpoint.Publish(message, context.CancellationToken);
            }

            _logger.LogInformation("Finished MoveImagesFromGoogleToS3 periodic job.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while executing MoveImagesFromGoogleToS3PeriodicWorker.");
        }
    }
}
