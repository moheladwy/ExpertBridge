using ExpertBridge.Api.EmbeddingService;
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
    private readonly OllamaEmbeddingService _embeddingService;


    public SearchController(
            ExpertBridgeDbContext dbContext,
            OllamaEmbeddingService embeddingService)
    {
        _dbContext = dbContext;
        _embeddingService = embeddingService;
    }

    // TODO: Test this endpoint before deploying.
    [HttpGet("posts")]
    public async Task<List<PostResponse>> SearchPosts(
            [FromQuery] string query,
            [FromQuery] int? limit = null,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(query, nameof(query));
        var embedding = await _embeddingService.GenerateEmbedding(query);
        if (embedding == null)
        {
            throw new InvalidOperationException("Failed to generate embedding for the query.");
        }
        var posts = await _dbContext.Posts
            .AsNoTracking()
            .Where(p => p.Embedding != null)
            .OrderBy(p => p.Embedding.CosineDistance(embedding))
            .Take(limit ?? 100)
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
