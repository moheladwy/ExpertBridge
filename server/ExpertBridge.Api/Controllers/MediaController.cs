using ExpertBridge.Application.Services;
using ExpertBridge.Core.Entities.Media.MediaGrants;
using ExpertBridge.Core.Requests.GeneratePresignedUrls;
using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpertBridge.Api.Controllers;

/// <summary>
///     This controller is used to handle media operations like uploading, downloading, and deleting objects from the s3
///     bucket.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MediaController : ControllerBase
{
    private readonly ExpertBridgeDbContext _dbContext;
    private readonly S3Service _s3Service;

    public MediaController(S3Service s3Service, ExpertBridgeDbContext dbContext)
    {
        _s3Service = s3Service;
        _dbContext = dbContext;
    }

    [HttpPost("generate-urls")] // generate-url?count=3
    public async Task<List<PresignedUrlResponse>> GenerateUrls([FromBody] GeneratePresignedUrlsRequest request)
    {
        if (request?.Files == null || request.Files.Count == 0)
        {
            throw new ArgumentException("Files cannot be null or empty", nameof(request));
        }

        var urls = new List<PresignedUrlResponse>();

        foreach (var file in request.Files)
        {
            urls.Add(await _s3Service.GetPresignedPutUrlAsync(file));
        }

        await _dbContext.MediaGrants.AddRangeAsync(
            urls.Select(url => new MediaGrant
            {
                Key = url.Key, GrantedAt = DateTime.UtcNow, IsActive = false, OnHold = true
            }));

        await _dbContext.SaveChangesAsync();

        return urls;
    }
}
