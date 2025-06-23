using System.Globalization;
using ExpertBridge.Api.EmbeddingService;
using ExpertBridge.Core.Queries;
using ExpertBridge.Core.Requests;
using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Pgvector.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace ExpertBridge.Api.Controllers;

/// <summary>
///     Handles search-related operations for the application. This controller is responsible for providing
///     endpoints related to retrieving posts based on specific search queries while utilizing embedded
///     data services and caching mechanisms.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SearchController : ControllerBase
{
    private readonly ExpertBridgeDbContext _dbContext;
    private readonly IEmbeddingService _embeddingService;
    private readonly HybridCache _cache;
    private readonly int _defaultLimit;
    private readonly float _cosineDistanceThreshold;

    public SearchController(
            ExpertBridgeDbContext dbContext,
            IEmbeddingService embeddingService,
            HybridCache cache)
    {
        _dbContext = dbContext;
        _embeddingService = embeddingService;
        _cache = cache;
        _cosineDistanceThreshold = 1.0f;
        _defaultLimit = 10;
    }

    [HttpGet("posts")]
    [AllowAnonymous]
    public async Task<List<PostResponse>> SearchPosts(
            [FromQuery] SearchPostRequest request,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        ArgumentException.ThrowIfNullOrEmpty(request.query, nameof(request.query));

        var queryEmbeddings = await _embeddingService.GenerateEmbedding(request.query);

        return await _dbContext.Posts
            .AsNoTracking()
            .Where(p => p.Embedding != null && p.Embedding.CosineDistance(queryEmbeddings) < _cosineDistanceThreshold)
            .OrderBy(p => p.Embedding.CosineDistance(queryEmbeddings))
            .Take(request.limit ?? _defaultLimit)
            .Select(p => new PostResponse
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                Author = p.Author.SelectAuthorResponseFromProfile(),
                CreatedAt = p.CreatedAt.Value,
                LastModified = p.LastModified,
                Upvotes = p.Votes.Count(v => v.IsUpvote),
                Downvotes = p.Votes.Count(v => !v.IsUpvote),
                Comments = p.Comments.Count,
                RelevanceScore = p.Embedding.CosineDistance(queryEmbeddings),
                Medias = p.Medias.Select(m => new MediaObjectResponse
                {
                    Id = m.Id,
                    Name = m.Name,
                    Type = m.Type,
                    Url = $"https://expert-bridge-media.s3.amazonaws.com/{m.Key}"
                }).ToList()
            })
            .ToListAsync(cancellationToken);
    }
}
