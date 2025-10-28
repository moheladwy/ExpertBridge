// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Contract.Messages;
using ExpertBridge.Data.DatabaseContexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace ExpertBridge.Worker.PeriodicJobs.UserInterestUpdater;

/// <summary>
///     Represents a periodic worker job responsible for updating the user interest embeddings
///     by publishing messages to a message broker for profiles that have not yet been processed.
/// </summary>
/// <remarks>
///     This worker job is executed as part of a Quartz job scheduler and processes user profile records
///     from the database that do not have their <c>UserInterestEmbedding</c> set. It publishes
///     <c>UserInterestsUpdatedMessage</c> messages for such profiles to a messaging endpoint.
/// </remarks>
/// <example>
///     This job uses dependency injection to access a database context, a logging provider, and a messaging
///     endpoint to accomplish its task. It retrieves profiles efficiently using query methods from Entity Framework.
/// </example>
/// <threadsafety>
///     This class is not thread-safe, as it depends on injected, potentially non-thread-safe dependencies.
///     However, execution is handled per-job instance and managed by Quartz.
/// </threadsafety>
/// <dependencies>
///     This class depends on the following components:
///     - <see cref="Microsoft.Extensions.Logging.ILogger" /> for logging errors and diagnostic information.
///     - <see cref="ExpertBridge.Data.DatabaseContexts.ExpertBridgeDbContext" /> for querying user profiles from the
///     database.
///     - <see cref="MassTransit.IPublishEndpoint" /> for publishing messages to the relevant topic.
/// </dependencies>
internal sealed class UserInterestUpdaterPeriodicWorker : IJob
{
    /// <summary>
    ///     Database context instance used for interacting with the database
    ///     to perform data access operations related to the execution of the
    ///     <see cref="UserInterestUpdaterPeriodicWorker" /> class.
    /// </summary>
    private readonly ExpertBridgeDbContext _dbContext;

    /// <summary>
    ///     Logger instance used for logging application events, errors, and information
    ///     related to the execution and behavior of the <see cref="UserInterestUpdaterPeriodicWorker" /> class.
    /// </summary>
    private readonly ILogger<UserInterestUpdaterPeriodicWorker> _logger;

    /// <summary>
    ///     The publish endpoint provided by MassTransit, used to send or publish messages
    ///     to the message bus, enabling communication between distributed components
    ///     within the <see cref="UserInterestUpdaterPeriodicWorker" /> class.
    /// </summary>
    private readonly IPublishEndpoint _publishEndpoint;

    /// <summary>
    ///     Represents a periodic job responsible for updating user interests in the system.
    ///     This worker executes specific logic at scheduled intervals and interacts with the database
    ///     and message bus for its operations.
    /// </summary>
    public UserInterestUpdaterPeriodicWorker(
        ILogger<UserInterestUpdaterPeriodicWorker> logger,
        ExpertBridgeDbContext dbContext,
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }

    /// <summary>
    ///     Executes the user interest updater job.
    /// </summary>
    /// <param name="context">The job execution context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method identifies profiles without user interest embeddings and publishes
    ///     messages to generate embeddings for interest-based matching.
    /// </remarks>
    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Starting User Interest Updater Periodic Worker.");

        var profiles = await _dbContext.Profiles
            .AsNoTracking()
            .Where(p => p.UserInterestEmbedding == null)
            .Select(p => new UserInterestsUpdatedMessage { UserProfileId = p.Id })
            .ToListAsync(context.CancellationToken);

        foreach (var profile in profiles)
        {
            try
            {
                await _publishEndpoint.Publish(profile, context.CancellationToken);
                _logger.LogInformation("Published user with Id {UserId} interests updated message.",
                    profile.UserProfileId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to send user interests to user interests updated channel.");
            }
        }

        _logger.LogInformation("User Interest Updater Periodic Worker completed.");
    }
}
