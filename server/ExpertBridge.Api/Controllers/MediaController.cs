using ExpertBridge.Api.Services;
using ExpertBridge.Contract.Requests.GeneratePresignedUrls;
using ExpertBridge.Contract.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpertBridge.Api.Controllers;

/// <summary>
///     This controller is used to handle media operations like
///     uploading, downloading, and deleting objects from the s3 bucket.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class MediaController : ControllerBase
{
    /// <summary>
    ///     A private, readonly instance of <see cref="MediaService" /> used for executing media-related operations,
    ///     such as generating presigned URLs, interacting with AWS S3, and managing media data within the database.
    ///     This instance is primarily utilized within the <see cref="MediaController" /> to handle incoming requests
    ///     related to media functionality.
    /// </summary>
    private readonly MediaService _mediaService;

    /// <summary>
    ///     The MediaController handles HTTP requests related to media operations such as
    ///     generating presigned URLs, and managing user interactions with objects stored in an S3 bucket.
    ///     This controller processes the data received via API calls and delegates actual processing
    ///     to the <see cref="MediaService" />.
    /// </summary>
    public MediaController(MediaService mediaService)
    {
        _mediaService = mediaService;
    }

    /// <summary>
    ///     Generates a list of presigned URLs for uploading files to AWS S3, using the metadata provided in the request.
    /// </summary>
    /// <param name="request">
    ///     The <see cref="GeneratePresignedUrlsRequest" /> containing metadata for the files to generate
    ///     presigned URLs for.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}" /> that represents the asynchronous operation, containing a list of
    ///     <see cref="PresignedUrlResponse" /> objects with the generated URLs and corresponding keys.
    /// </returns>
    [HttpPost("generate-urls")] // generate-url?count=3
    public async Task<List<PresignedUrlResponse>> GenerateUrls([FromBody] GeneratePresignedUrlsRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return await _mediaService.GenerateUrls(request);
    }
}
