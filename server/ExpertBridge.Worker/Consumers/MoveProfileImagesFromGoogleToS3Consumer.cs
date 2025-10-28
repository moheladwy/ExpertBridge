using Amazon.S3.Model;
using ExpertBridge.Application.Services;
using ExpertBridge.Contract.Messages;
using ExpertBridge.Data.DatabaseContexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Worker.Consumers;

/// <summary>
///     Consumer class for handling the process of moving profile images from Google Storage to Amazon S3.
/// </summary>
/// <remarks>
///     This class listens for messages of type <see cref="MoveProfileImageFromGoogleToS3Message" /> and processes
///     them by downloading profile images from their source in Google Cloud Storage and uploading them to Amazon S3.
///     It ensures proper logging, error handling, and database updates during the operation.
/// </remarks>
public sealed class MoveProfileImagesFromGoogleToS3Consumer : IConsumer<MoveProfileImageFromGoogleToS3Message>
{
    /// <summary>
    ///     Instance of the <see cref="ExpertBridgeDbContext" /> used to interact with the database
    ///     for operations required by the <see cref="MoveProfileImagesFromGoogleToS3Consumer" /> consumer.
    /// </summary>
    /// <remarks>
    ///     This database context manages persistence and retrieval of data associated with the task of
    ///     migrating profile images from Google Cloud Storage to Amazon S3. It handles entity tracking,
    ///     change detection, and database queries in the context of this operation.
    /// </remarks>
    private readonly ExpertBridgeDbContext _dbContext;

    /// <summary>
    ///     Factory for creating <see cref="HttpClient" /> instances, used in the
    ///     <see cref="MoveProfileImagesFromGoogleToS3Consumer" /> consumer for executing HTTP requests to external services
    ///     such as downloading profile images from Google Cloud.
    /// </summary>
    /// <remarks>
    ///     This factory is leveraged to ensure proper HttpClient instance management, avoiding common issues
    ///     like socket exhaustion, and adhering to the recommended usage patterns for making HTTP requests in .NET.
    /// </remarks>
    private readonly IHttpClientFactory _httpClientFactory;

    /// <summary>
    ///     Instance of the <see cref="ILogger{TCategoryName}" /> for logging application flow and errors
    ///     within the <see cref="MoveProfileImagesFromGoogleToS3Consumer" /> consumer.
    /// </summary>
    /// <remarks>
    ///     This logger is used to log informational messages, warnings, and errors that occur while
    ///     processing the task of moving profile images from Google Cloud Storage to Amazon S3.
    /// </remarks>
    private readonly ILogger<MoveProfileImagesFromGoogleToS3Consumer> _logger;

    /// <summary>
    ///     Instance of the <see cref="S3Service" /> used for interacting with Amazon S3 storage
    ///     to facilitate the migration of profile images from Google Cloud Storage.
    /// </summary>
    /// <remarks>
    ///     This service is responsible for handling operations such as uploading images to S3,
    ///     generating signed URLs for uploaded objects, and managing metadata during the
    ///     process of profile image migration in the <see cref="MoveProfileImagesFromGoogleToS3Consumer" /> consumer.
    /// </remarks>
    private readonly S3Service _s3Service;

    /// <summary>
    ///     Processes messages related to moving user profile images from Google Cloud Storage to AWS S3.
    /// </summary>
    /// <remarks>
    ///     This class leverages injected services such as logging, database context, HTTP client factory,
    ///     and the S3 service to handle image migration. It implements the IConsumer interface to handle
    ///     messages that trigger the migration operation.
    /// </remarks>
    public MoveProfileImagesFromGoogleToS3Consumer(
        ILogger<MoveProfileImagesFromGoogleToS3Consumer> logger,
        ExpertBridgeDbContext dbContext,
        S3Service s3Service,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _dbContext = dbContext;
        _s3Service = s3Service;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    ///     Handles the consumption of MoveProfileImageFromGoogleToS3Message to migrate user profile images
    ///     from Google Cloud Storage to AWS S3.
    /// </summary>
    /// <param name="context">
    ///     The consumption context containing the MoveProfileImageFromGoogleToS3Message
    ///     with details about the profile image to be migrated.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation of processing the message,
    ///     including downloading the image, uploading it to S3, and updating the profile record.
    /// </returns>
    public async Task Consume(ConsumeContext<MoveProfileImageFromGoogleToS3Message> context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        var message = context.Message;
        _logger.LogInformation("Starting to move profile image for ProfileId: {ProfileId}", message.ProfileId);

        try
        {
            // var profile = await _dbContext.Profiles.FindAsync(message.ProfileId);
            var pictureUrl = await _dbContext.Profiles
                .AsNoTracking()
                .Where(p => p.Id == message.ProfileId)
                .Select(p => p.ProfilePictureUrl)
                .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(pictureUrl))
            {
                _logger.LogInformation("No Google profile image URL found for ProfileId: {ProfileId}",
                    message.ProfileId);
                return;
            }

            if (!pictureUrl.Contains("googleusercontent.com", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("Profile image for ProfileId: {ProfileId} is not hosted on Google. Skipping.",
                    message.ProfileId);
                return;
            }

            // Download image from Google
            _logger.LogInformation("Downloading image from Google for ProfileId: {ProfileId}", message.ProfileId);
            using var httpClient = _httpClientFactory.CreateClient();
            var imageResponse = await httpClient.GetAsync(new Uri(pictureUrl));

            if (!imageResponse.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Failed to download image from Google for ProfileId: {ProfileId}. Status: {StatusCode}",
                    message.ProfileId, imageResponse.StatusCode);
                return;
            }

            // Read the image stream
            await using var imageStream = await imageResponse.Content.ReadAsStreamAsync();
            var contentType = imageResponse.Content.Headers.ContentType?.MediaType ?? "image/jpeg";

            // Upload to S3
            _logger.LogInformation("Uploading image to S3 for ProfileId: {ProfileId}", message.ProfileId);
            var putRequest = new PutObjectRequest
            {
                InputStream = imageStream,
                ContentType = contentType,
                Metadata =
                {
                    ["file-name"] = $"profile-{message.ProfileId}",
                    ["original-url"] = pictureUrl
                }
            };

            var uploadResponse = await _s3Service.UploadObjectAsync(putRequest);

            await _dbContext.Profiles
                .Where(p => p.Id == message.ProfileId)
                .ExecuteUpdateAsync(calls =>
                    calls.SetProperty(p => p.ProfilePictureUrl, uploadResponse.FileUrl));

            _logger.LogInformation(
                "Successfully migrated profile image for ProfileId: {ProfileId} to S3. New URL: {S3Url}",
                message.ProfileId, uploadResponse.FileUrl);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex,
                "Invalid operation when migrating profile image for ProfileId: {ProfileId}. " +
                "The requestUri must be an absolute URI or BaseAddress must be set.",
                message.ProfileId);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex,
                "HTTP request failed when migrating profile image for ProfileId: {ProfileId}. " +
                "The request failed due to an underlying issue such as network connectivity, DNS failure, " +
                "server certificate validation or timeout.",
                message.ProfileId);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex,
                "Request timeout when migrating profile image for ProfileId: {ProfileId}. " +
                "The request failed due to timeout.",
                message.ProfileId);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex,
                "Database concurrency violation when migrating profile image for ProfileId: {ProfileId}. " +
                "A concurrency violation occurs when an unexpected number of rows are affected during save. " +
                "This is usually because the data in the database has been modified since it was loaded into memory.",
                message.ProfileId);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex,
                "Database update error when migrating profile image for ProfileId: {ProfileId}. " +
                "An error is encountered while saving to the database.",
                message.ProfileId);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex,
                "Operation cancelled when migrating profile image for ProfileId: {ProfileId}. " +
                "The CancellationToken is canceled.",
                message.ProfileId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error migrating profile image for ProfileId: {ProfileId}",
                message.ProfileId);
        }
    }
}
