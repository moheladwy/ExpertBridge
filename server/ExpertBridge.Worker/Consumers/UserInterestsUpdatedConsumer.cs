// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text;
using ExpertBridge.Application.EmbeddingService;
using ExpertBridge.Core.Exceptions;
using ExpertBridge.Core.Messages;
using ExpertBridge.Data.DatabaseContexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Worker.Consumers;

/// <summary>
///     A consumer that processes the <c>UserInterestsUpdatedMessage</c> message.
/// </summary>
/// <remarks>
///     This consumer is responsible for updating user interest embeddings. It retrieves
///     the user interests from the database, generates an embedding using the <c>IEmbeddingService</c>,
///     and updates the user profile information if necessary.
/// </remarks>
/// <example>
///     This class is used as part of a message consumption pipeline implemented with MassTransit.
///     It listens for events indicating that user interests have been updated and processes the corresponding updates.
/// </example>
/// <remarks>
///     Handles logging of processing information and errors during the consumption of the message.
/// </remarks>
public sealed class UserInterestsUpdatedConsumer : IConsumer<UserInterestsUpdatedMessage>
{
    /// <summary>
    ///     Database context for accessing and managing persistent storage related to the ExpertBridge domain.
    ///     This instance facilitates communication with the underlying database for CRUD operations,
    ///     data retrieval, and transactional updates across entities within the system, including
    ///     profiles, user interests, and related domain models.
    ///     Typical usage includes querying user interests, profiles, or related data for processing
    ///     and updating embedding information or saving changes back to the database.
    /// </summary>
    private readonly ExpertBridgeDbContext _dbContext;

    /// <summary>
    ///     Instance of <see cref="IEmbeddingService" /> used to handle the generation of embeddings
    ///     for updated user interests in the <see cref="UserInterestsUpdatedConsumer" /> class. This service
    ///     is essential for processing new or modified user profile data by converting user interests
    ///     into suitable embeddings, which are later used for further analysis or storage in the database.
    ///     Typically invoked to:
    ///     - Generate embeddings based on descriptive textual data derived from user interests.
    ///     - Support updates to the user profile after embedding calculations are completed.
    ///     - Enable downstream systems to leverage the processed embeddings for recommendations,
    ///     search, or analytics.
    /// </summary>
    private readonly IEmbeddingService _embeddingService;

    /// <summary>
    ///     Logger instance used to log informational, warning, error, and debugging messages
    ///     during the processing of user interests updates in the consumer. This logger
    ///     specifically helps track the flow of operations, report issues, and provide insights
    ///     into the embedding generation and database update workflow.
    ///     Typically used to log messages when:
    ///     - Updating user interests embedding.
    ///     - Detecting anomalies or errors in the embedding generation process.
    ///     - Notifying about the progress of database updates.
    ///     - Capturing key details such as user profile IDs for traceability.
    ///     The logger is configured to target instances of the <see cref="UserInterestsUpdatedConsumer" /> class.
    /// </summary>
    private readonly ILogger<UserInterestsUpdatedConsumer> _logger;

    /// <summary>
    ///     Consumer for handling UserInterestsUpdatedMessage events.
    /// </summary>
    /// <remarks>
    ///     This consumer listens for messages of type UserInterestsUpdatedMessage and performs actions based on the received
    ///     event.
    ///     It is part of the messaging system for reacting to updates in user interests.
    /// </remarks>
    public UserInterestsUpdatedConsumer(
        ILogger<UserInterestsUpdatedConsumer> logger,
        ExpertBridgeDbContext dbContext,
        IEmbeddingService embeddingService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _embeddingService = embeddingService;
    }

    /// <summary>
    ///     Processes the UserInterestsUpdatedMessage to handle updates in user interest embeddings.
    /// </summary>
    /// <param name="context">The context of the consumed UserInterestsUpdatedMessage, containing message data and metadata.</param>
    /// <returns>A task that represents the asynchronous operation of processing the message.</returns>
    public async Task Consume(ConsumeContext<UserInterestsUpdatedMessage> context)
    {
        try
        {
            var message = context.Message;
            _logger.LogInformation("Updating user interests embedding for user profile {UserProfileId}",
                message.UserProfileId);

            var userInterests = _dbContext.UserInterests
                .AsNoTracking()
                .Include(ui => ui.Tag)
                .Where(ui => ui.ProfileId == message.UserProfileId)
                .Select(ui => $"{ui.Tag.EnglishName} {ui.Tag.ArabicName} {ui.Tag.Description} ");

            var text = new StringBuilder();
            foreach (var ui in userInterests)
            {
                text.Append(ui);
            }

            var embedding = await _embeddingService.GenerateEmbedding(text.ToString());

            if (embedding is null)
            {
                _logger.LogError("Embedding service returned null embedding for user={UserProfileId}",
                    message.UserProfileId);
                throw new RemoteServiceCallFailedException(
                    $"Error: Embedding service returned null embedding for user=${message.UserProfileId}.");
            }

            _logger.LogInformation("Updated user interests embedding for user profile {UserProfileId}",
                message.UserProfileId);

            var user = await _dbContext.Profiles
                .FirstOrDefaultAsync(p => p.Id == message.UserProfileId, context.CancellationToken);

            if (user is not null)
            {
                user.UserInterestEmbedding = embedding;
                await _dbContext.SaveChangesAsync(context.CancellationToken);
                _logger.LogInformation("Database changes saved for user profile {UserProfileId}",
                    message.UserProfileId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An error occurred while processing message with user profile id={Id}.",
                context.Message.UserProfileId);
        }
    }
}
