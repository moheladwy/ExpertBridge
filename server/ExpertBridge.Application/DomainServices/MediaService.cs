// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Application.Services;
using ExpertBridge.Contract.Requests.GeneratePresignedUrls;
using ExpertBridge.Contract.Responses;
using ExpertBridge.Core.Entities.Media.MediaGrants;
using ExpertBridge.Data.DatabaseContexts;
using FluentValidation;

namespace ExpertBridge.Application.DomainServices;

/// <summary>
///     Provides functionality related to media services, such as generating presigned URLs for file uploads
///     and managing media-related data within the database.
///     This service interacts with AWS S3 for presigned URL generation and a database context for storing
///     media grant data.
/// </summary>
public sealed class MediaService
{
    /// <summary>
    ///     Represents the database context used for interacting with the application's underlying database.
    ///     This instance is specific to the `ExpertBridgeDbContext`, which provides access to entities such as
    ///     Users, Profiles, and other domain models. It is used for querying and persisting data in the database.
    /// </summary>
    private readonly ExpertBridgeDbContext _dbContext;

    /// <summary>
    ///     Validates instances of <see cref="GeneratePresignedUrlsRequest" /> to ensure that all required
    ///     data is correctly provided for generating presigned URLs for S3 file uploads.
    ///     This validator performs checks on the request structure, such as validating file metadata, ensuring
    ///     compliance with expected constraints, and preventing malformed data from being processed.
    /// </summary>
    private readonly IValidator<GeneratePresignedUrlsRequest> _generatePresignedUrlsValidator;

    /// <summary>
    ///     Represents the service responsible for interacting with an S3-compatible storage system.
    ///     Used for operations such as generating presigned URLs for file uploads, downloads,
    ///     and managing file objects within the MediaService class.
    /// </summary>
    private readonly S3Service _s3Service;

    /// <summary>
    ///     Provides services for handling media-related operations within the application domain.
    /// </summary>
    public MediaService(
        ExpertBridgeDbContext dbContext,
        S3Service s3Service,
        IValidator<GeneratePresignedUrlsRequest> generatePresignedUrlsValidator)
    {
        _dbContext = dbContext;
        _s3Service = s3Service;
        _generatePresignedUrlsValidator = generatePresignedUrlsValidator;
    }

    /// <summary>
    ///     Generates a list of presigned URLs for file uploads based on the given request data.
    /// </summary>
    /// <param name="request">
    ///     The request object containing metadata about the files for which presigned URLs should be
    ///     generated.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a list of presigned URL responses
    ///     for the specified files.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when the provided <paramref name="request" /> is null.</exception>
    /// <exception cref="ValidationException">Thrown when the provided <paramref name="request" /> fails validation.</exception>
    public async Task<List<PresignedUrlResponse>> GenerateUrls(GeneratePresignedUrlsRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        var validationResult = await _generatePresignedUrlsValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var urls = new List<PresignedUrlResponse>();

        foreach (var file in request.Files)
        {
            urls.Add(await _s3Service.GetPresignedPutUrlAsync(file));
        }

        await _dbContext.MediaGrants.AddRangeAsync(urls.Select(url => new MediaGrant
        {
            Key = url.Key, GrantedAt = DateTime.UtcNow, IsActive = false, OnHold = true
        }));

        await _dbContext.SaveChangesAsync();

        return urls;
    }
}
