// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Contract.Messages;
using ExpertBridge.Core.Entities.ManyToManyRelationships.UserInterests;
using ExpertBridge.Core.Entities.Tags;
using ExpertBridge.Core.Exceptions;
using ExpertBridge.Data.DatabaseContexts;
using ExpertBridge.Worker.Services;
using MassTransit;

namespace ExpertBridge.Worker.Consumers;

/// <summary>
///     Consumer responsible for processing user interest tags by translating them,
///     categorizing them, and persisting the resulting tags and user interests in the database.
/// </summary>
/// <remarks>
///     This class listens for messages of type <see cref="UserInterestsProsessingMessage" /> using MassTransit
///     and utilizes the <see cref="AiTagProcessorService" /> for tag translation and categorization.
///     The processed tags and user interests are saved into the database using <see cref="ExpertBridgeDbContext" />.
/// </remarks>
public sealed class UserInterestsTagsProcessingConsumer : IConsumer<UserInterestsProsessingMessage>
{
    /// <summary>
    ///     Represents the service used to process and translate user interest tags
    ///     within the <see cref="UserInterestsTagsProcessingConsumer" /> class.
    /// </summary>
    /// <remarks>
    ///     This service performs tag processing, including translation and categorization,
    ///     enabling the conversion of raw user interest tag data into structured tags for further use.
    /// </remarks>
    private readonly AiTagProcessorService _aiTagProcessorService;

    /// <summary>
    ///     Represents the database context used for accessing and managing the application's database entities
    ///     within the <see cref="UserInterestsTagsProcessingConsumer" /> class.
    /// </summary>
    /// <remarks>
    ///     This context is utilized for persisting processed user interests and tags into the database.
    ///     It provides access to the application's database entities by leveraging the <see cref="ExpertBridgeDbContext" />.
    /// </remarks>
    private readonly ExpertBridgeDbContext _dbContext;

    /// <summary>
    ///     Represents a logger instance used to log information, warnings, and errors
    ///     within the <see cref="UserInterestsTagsProcessingConsumer" /> class.
    /// </summary>
    /// <remarks>
    ///     This logger is used for logging messages related to user interest tag processing,
    ///     such as successes, errors, and service call failures.
    /// </remarks>
    private readonly ILogger<UserInterestsTagsProcessingConsumer> _logger;

    /// <summary>
    ///     A consumer class responsible for processing messages related to user interest tags.
    ///     It integrates with the message consuming mechanism to handle user interest tag processing logic.
    /// </summary>
    /// <param name="logger">The logger instance for logging information and errors.</param>
    /// <param name="aiTagProcessorService">The service used for processing and translating tags.</param>
    /// <param name="dbContext">The database context for accessing and managing database entities.</param>
    public UserInterestsTagsProcessingConsumer(
        ILogger<UserInterestsTagsProcessingConsumer> logger,
        AiTagProcessorService aiTagProcessorService,
        ExpertBridgeDbContext dbContext)
    {
        _logger = logger;
        _aiTagProcessorService = aiTagProcessorService;
        _dbContext = dbContext;
    }

    /// <summary>
    ///     Processes a message of type <see cref="UserInterestsProsessingMessage" /> by translating, categorizing,
    ///     and saving user interest tags into the database.
    /// </summary>
    /// <param name="context">The consume context containing the message of type <see cref="UserInterestsProsessingMessage" />.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method handles the processing of user interest tags by utilizing the <see cref="AiTagProcessorService" />
    ///     for translation and categorization. The resulting tags and user interests are then persisted into the database
    ///     using the <see cref="ExpertBridgeDbContext" />.
    /// </remarks>
    /// <exception cref="RemoteServiceCallFailedException">
    ///     Thrown when the tag processor service returns a null or empty result.
    /// </exception>
    public async Task Consume(ConsumeContext<UserInterestsProsessingMessage> context)
    {
        try
        {
            var message = context.Message;
            _logger.LogInformation("Processing user interests tags for user profile {UserProfileId}",
                message.UserProfileId);

            var results = await _aiTagProcessorService
                .TranslateTagsAsync(message.InterestsTags);

            if (results == null)
            {
                _logger.LogError("Error: Tag processor service returned null result for user={UserProfileId}.",
                    message.UserProfileId);
                throw new RemoteServiceCallFailedException(
                    $"Error: Tag processor service returned null result for user={message.UserProfileId}.");
            }

            var categorizerTags = results.Tags;
            if (categorizerTags.Count == 0)
            {
                _logger.LogError("Error: Tag processor service returned empty result for user={UserProfileId}.",
                    message.UserProfileId);
                throw new RemoteServiceCallFailedException(
                    $"Error: Tag processor service returned empty result for user={message.UserProfileId}.");
            }


            var tags = categorizerTags
                .Select(result => new Tag
                {
                    ArabicName = result.ArabicName,
                    EnglishName = result.EnglishName,
                    Description = result.Description
                }).ToList();

            await _dbContext.Tags.AddRangeAsync(tags, context.CancellationToken);

            await _dbContext.UserInterests
                .AddRangeAsync(
                    tags.Select(tag => new UserInterest { ProfileId = message.UserProfileId, Tag = tag }).ToList(),
                    context.CancellationToken);

            await _dbContext.SaveChangesAsync(context.CancellationToken);
            _logger.LogInformation("Database changes saved for user profile {UserProfileId}", message.UserProfileId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error processing user interests tags for user profile {UserProfileId}, Exception: {Exception}",
                context.Message.UserProfileId,
                ex.Message);
        }
    }
}
