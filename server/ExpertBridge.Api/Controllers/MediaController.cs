using ExpertBridge.Api.Helpers;
using ExpertBridge.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpertBridge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MediaController(IObjectStorageService objectStorageService) : ControllerBase
{
    [HttpGet("download/{key}")]
    public async Task<IActionResult> DownloadObjectAsync([FromRoute] string key)
    {
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));
        var response = await objectStorageService.GetObjectAsync(key);
        return File(response.ResponseStream, response.ContentType, response.FileName);
    }

    [HttpGet("url/{key}")]
    public async Task<IActionResult> GetObjectUrlAsync([FromRoute] string key)
    {
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));
        var url = await objectStorageService.GetObjectUrlAsync(key);
        return Ok(url);
    }

    [HttpGet("presigned-url/{key}")]
    public async Task<IActionResult> GetPresignedUrlAsync([FromRoute] string key)
    {
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));
        var url = await objectStorageService.GetPresignedUrlAsync(key);
        return Ok(url);
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadObjectAsync(IFormFile file)
    {
        ArgumentNullException.ThrowIfNull(file, nameof(file));
        if (file.Length == 0) return BadRequest("File is empty");

        var s3ObjectRequest = await file.ToPutObjectRequestAsync();
        var response = await objectStorageService.UploadObjectAsync(s3ObjectRequest);

        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("delete/{key}")]
    public async Task<IActionResult> DeleteObjectAsync([FromRoute] string key)
    {
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));
        await objectStorageService.DeleteObjectAsync(key);
        return Ok("File deleted successfully");
    }
}
