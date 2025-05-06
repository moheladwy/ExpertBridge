using Api.Services;
using Core.Entities.Media.MediaGrants;
using Core.Requests;
using Core.Responses;
using Data.DatabaseContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

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
    private readonly ExpertBridgeDbContext _dbContext;

    public MediaController(S3Service s3Service, ExpertBridgeDbContext dbContext)
    {
        _s3Service = s3Service;
        _dbContext = dbContext;
    }

    [HttpPost("generate-urls")] // generate-url?count=3
    public async Task<List<PresignedUrlResponse>> GenerateUrls([FromBody] GeneratePresignedUrlsRequest request)
    {
        if (request == null || request.Files == null || request.Files.Count == 0)
            throw new ArgumentException("Files cannot be null or empty", nameof(request));

        var urls = new List<PresignedUrlResponse>();

        foreach (var file in request.Files)
        {
            urls.Add(await _s3Service.GetPresignedPutUrlAsync(file));
        }

        await _dbContext.MediaGrants.AddRangeAsync(
            urls.Select(url => new MediaGrant
            {
                Key = url.Key,
                GrantedAt = DateTime.UtcNow,
                IsActive = false,
                OnHold = true
            }));

        await _dbContext.SaveChangesAsync();

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
