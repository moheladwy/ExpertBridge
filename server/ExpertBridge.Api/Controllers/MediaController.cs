// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Amazon.S3.Model;
using ExpertBridge.Api.Core.Interfaces.Services;
using ExpertBridge.Api.Helpers;
using ExpertBridge.Api.Responses;
using ExpertBridge.Api.Services;
using ExpertBridge.Api.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpertBridge.Api.Controllers;

/// <summary>
///     This controller is used to handle media operations like uploading, downloading, and deleting objects from the s3 bucket.
/// </summary>
/// <param name="objectStorageService">
///     The service that handles the operations on the s3 bucket.
/// </param>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MediaController : ControllerBase
{
    private readonly S3Service _s3Service;

    public MediaController(S3Service s3Service)
    {
        _s3Service = s3Service;
    }

    // TODO: Remove AllowAnonymous
    [AllowAnonymous]
    [HttpGet("generate-urls")] // generate-url?count=3
    public async Task<List<PresignedUrlResponse>> GenerateUrls([FromQuery] int? count)
    {
        if (count is null or <= 0) count = 1;

        var urls = new List<PresignedUrlResponse>();

        for (var i = 0; i < count; i++)
        {
            var url = await _s3Service.GetPresignedPutUrlAsync();
            urls.Add(url);
        }

        return urls;
    }

    /// <summary>
    ///     This endpoint is used to download an object from the s3 bucket.
    /// </summary>
    /// <param name="key">
    ///     The key of the object to download.
    /// </param>
    /// <returns>
    ///     The object itself as a file.
    /// </returns>
    //[HttpGet("download/{key}")]
    //public async Task<IActionResult> DownloadObjectAsync([FromRoute] string key)
    //{
    //    ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));
    //    var response = await objectStorageService.GetObjectAsync(key);
    //    return File(response.ResponseStream, response.ContentType, response.FileName);
    //}

    /// <summary>
    ///     This endpoint is used to get the url of an object in the s3 bucket.
    /// </summary>
    /// <param name="key">
    ///     The key of the object to get the url of.
    /// </param>
    /// <returns>
    ///     The url of the object in the s3 bucket to download it from the client side.
    /// </returns>
    //[HttpGet("url/{key}")]
    //public async Task<GetMediaUrlResponse> GetObjectUrlAsync([FromRoute] string key)
    //{
    //    ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));
    //    return await objectStorageService.GetObjectUrlAsync(key);
    //}

    /// <summary>
    ///     This endpoint is used to upload an object to the s3 bucket.
    /// </summary>
    /// <param name="file">
    ///     The file to upload to the s3 bucket.
    /// </param>
    /// <returns>
    ///     A message indicating that the file was uploaded successfully.
    /// </returns>
    //[HttpPost("upload")]
    //public async Task<UploadFileResponse> UploadObjectAsync(IFormFile file)
    //{
    //    ArgumentNullException.ThrowIfNull(file, nameof(file));
    //    if (file.Length == 0) throw new ArgumentException("File is empty", nameof(file));

    //    var s3ObjectRequest = await file.ToPutObjectRequestAsync();
    //    return await objectStorageService.UploadObjectAsync(s3ObjectRequest);
    //}

    /// <summary>
    ///     This endpoint is used to delete an object from the s3 bucket.
    /// </summary>
    /// <param name="key">
    ///     The key of the object to delete.
    /// </param>
    /// <returns>
    ///     A message indicating that the file was deleted successfully.
    /// </returns>
    //[HttpDelete("delete/{key}")]
    //public async Task<IActionResult> DeleteObjectAsync([FromRoute] string key)
    //{
    //    ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));
    //    await objectStorageService.DeleteObjectAsync(key);
    //    return Ok("File deleted successfully");
    //}
}
