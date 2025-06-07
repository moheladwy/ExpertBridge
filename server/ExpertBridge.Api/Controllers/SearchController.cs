using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;

namespace ExpertBridge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SearchController : ControllerBase
{
    private readonly ExpertBridgeDbContext _dbContext;

    public SearchController(ExpertBridgeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("posts")]
    [AllowAnonymous]
    public async Task<List<PostResponse>> SearchPosts(
            [FromQuery] string query,
            [FromQuery] int? limit = null,
            CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(query, nameof(query));
        var posts = await _dbContext.Posts
            .AsNoTracking()
            .Where(p => p.Embedding != null)
            .OrderBy(p => p.Embedding.CosineDistance(query))
            .Take(limit ?? 10)
            .Select(p => new PostResponse
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
            })
            .ToListAsync(cancellationToken);
        return posts ?? [];
    }
}
