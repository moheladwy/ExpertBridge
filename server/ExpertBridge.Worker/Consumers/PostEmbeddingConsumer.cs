// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Application.EmbeddingService;
using ExpertBridge.Contract.Messages;
using ExpertBridge.Core.Entities.JobPostings;
using ExpertBridge.Core.Exceptions;
using ExpertBridge.Core.Interfaces;
using ExpertBridge.Data.DatabaseContexts;
using ExpertBridge.Notifications;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;

namespace ExpertBridge.Worker.Consumers;

/// <summary>
///     Implements the consumer for processing <see cref="EmbedPostMessage" /> messages.
///     This class listens for embedding-related events and performs the necessary operations,
///     such as generating embeddings for post content and updating the database accordingly.
/// </summary>
/// <remarks>
///     The consumer integrates with various services:
///     - Embedding generation is handled by the <see cref="IEmbeddingService" />.
///     - Database context operations are performed with <see cref="ExpertBridgeDbContext" />.
///     - Logging is managed using <see cref="ILogger{PostEmbeddingConsumer}" />.
///     - Notifications are managed using <see cref="NotificationFacade" />.
///     This class handles both job postings and general posts.
/// </remarks>
/// <exception cref="RemoteServiceCallFailedException">
///     Thrown when the embedding service returns a null embedding.
/// </exception>
/// <seealso cref="EmbedPostMessage" />
/// <seealso cref="ExpertBridgeDbContext" />
/// <seealso cref="IEmbeddingService" />
internal sealed class PostEmbeddingConsumer : IConsumer<EmbedPostMessage>
{
    /// <summary>
    ///     Provides database access functionality for the <see cref="PostEmbeddingConsumer" /> class.
    /// </summary>
    /// <remarks>
    ///     This variable facilitates querying and updating the data stored in the application's database
    ///     during the processing of <see cref="EmbedPostMessage" /> messages. It is an instance of
    ///     <see cref="ExpertBridgeDbContext" />, which serves as the primary interface for interacting
    ///     with database entities such as job postings, posts, and user profiles.
    /// </remarks>
    /// <seealso cref="ExpertBridgeDbContext" />
    private readonly ExpertBridgeDbContext _dbContext;

    /// <summary>
    ///     Provides access to embedding generation functionalities for posts processed by
    ///     the <see cref="PostEmbeddingConsumer" /> class.
    /// </summary>
    /// <remarks>
    ///     This variable is used to interact with an implementation of <see cref="IEmbeddingService" />
    ///     to generate embeddings based on the content of posts. The embeddings are utilized to support
    ///     advanced content recommendation and similarity matching functionalities within the application.
    /// </remarks>
    /// <seealso cref="IEmbeddingService" />
    private readonly IEmbeddingService _embeddingService;

    /// <summary>
    ///     Provides logging functionality for the <see cref="PostEmbeddingConsumer" /> class.
    /// </summary>
    /// <remarks>
    ///     This variable is used to log information, warnings, errors, and other diagnostic messages
    ///     during the processing of <see cref="EmbedPostMessage" /> messages.
    ///     It is an instance of <see cref="ILogger{PostEmbeddingConsumer}" />, which integrates with the
    ///     application's logging framework for structured and leveled logging.
    /// </remarks>
    /// <seealso cref="ILogger{PostEmbeddingConsumer}" />
    private readonly ILogger<PostEmbeddingConsumer> _logger;

    /// <summary>
    ///     Facilitates notification functionality for the <see cref="PostEmbeddingConsumer" /> class.
    /// </summary>
    /// <remarks>
    ///     This variable is used to notify users about job matches or other relevant updates, leveraging
    ///     the capabilities provided by the <see cref="NotificationFacade" /> class.
    ///     It acts as an interface for sending notifications within the application, encapsulating
    ///     the logic required for dispatching user-facing messages.
    /// </remarks>
    /// <seealso cref="NotificationFacade" />
    private readonly NotificationFacade _notifications;

    /// <summary>
    ///     A consumer for processing embedded post-related messages within
    ///     the application. This class is responsible for handling messages of type
    ///     <see cref="EmbedPostMessage" /> and interacting with relevant services
    ///     such as embedding services, database contexts, and notification systems.
    /// </summary>
    public PostEmbeddingConsumer(
        ILogger<PostEmbeddingConsumer> logger,
        IEmbeddingService embeddingService,
        ExpertBridgeDbContext dbContext,
        NotificationFacade notifications)
    {
        _logger = logger;
        _embeddingService = embeddingService;
        _dbContext = dbContext;
        _notifications = notifications;
    }

    /// <summary>
    ///     Consumes and processes the <see cref="EmbedPostMessage" /> instance by generating
    ///     an embedding for the post, updating relevant database entities, and sending notifications
    ///     if applicable. Handles exceptions and logs errors during the message processing lifecycle.
    /// </summary>
    /// <param name="context">
    ///     The consumption context containing the <see cref="EmbedPostMessage" />
    ///     and additional metadata for processing.
    /// </param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation of the message consumption.</returns>
    public async Task Consume(ConsumeContext<EmbedPostMessage> context)
    {
        try
        {
            var post = context.Message;
            _logger.LogDebug("Generating embedding for PostId: {PostId}", post.PostId);
            var embedding = await _embeddingService.GenerateEmbedding($"{post.Title} {post.Content}");

            if (embedding is null)
            {
                _logger.LogError("Embedding service returned null embedding for PostId={PostId}", post.PostId);
                throw new RemoteServiceCallFailedException(
                    $"Error: Embedding service returned null embedding for post=${post.PostId}.");
            }

            IRecommendableContent? existingPost = null;

            if (post.IsJobPosting)
            {
                existingPost = await _dbContext.JobPostings
                    .FirstOrDefaultAsync(p => p.Id == post.PostId, context.CancellationToken);
            }
            else
            {
                existingPost = await _dbContext.Posts
                    .FirstOrDefaultAsync(p => p.Id == post.PostId, context.CancellationToken);
            }

            if (existingPost is null)
            {
                _logger.LogWarning("No post found with Id={PostId}. Skipping moderation.", post.PostId);
                return;
            }

            existingPost.Embedding = embedding;
            _logger.LogDebug("Embedding generated for PostId={PostId}", post.PostId);
            await _dbContext.SaveChangesAsync(context.CancellationToken);

            if (post.IsJobPosting)
            {
                // CONSIDER! Not the most effecient query due to the duplicate calculation of CosineDistance.
                // But should not be that heavy on the database considering the relatively small number of users
                // compared to the number of posts.

                _logger.LogDebug("Looking up candidates for JobPosting with Id={PostId}", post.PostId);
                var candidates = await _dbContext.Profiles
                    .Where(p => p.UserInterestEmbedding != null &&
                                embedding.CosineDistance(p.UserInterestEmbedding) < 1.0)
                    .OrderBy(p => embedding.CosineDistance(p.UserInterestEmbedding))
                    .ToListAsync(context.CancellationToken);

                _logger.LogDebug("Notifying candidates for JobPosting with Id={PostId}", post.PostId);
                await _notifications.NotifyJobMatchAsync((JobPosting)existingPost!, candidates);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error processing EmbedPostMessage: {ErrorMessage}", e.Message);
        }
    }
}
