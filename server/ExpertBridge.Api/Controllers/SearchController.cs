using System.Globalization;
using ExpertBridge.Application.DomainServices;
using ExpertBridge.Application.EmbeddingService;
using ExpertBridge.Core.Queries;
using ExpertBridge.Core.Requests.SearchJobPosts;
using ExpertBridge.Core.Requests.SearchPost;
using ExpertBridge.Core.Requests.SearchUser;
using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;

namespace ExpertBridge.Api.Controllers;

/// <summary>
///     Handles search-related operations for the application. This controller is responsible for providing
///     endpoints related to retrieving posts based on specific search queries while utilizing embedded
///     data services and caching mechanisms.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class SearchController : ControllerBase
{
    private readonly float _cosineDistanceThreshold;
    private readonly ExpertBridgeDbContext _dbContext;
    private readonly int _defaultLimit;
    private readonly IEmbeddingService _embeddingService;
    private readonly UserService _userService;

    public SearchController(
        ExpertBridgeDbContext dbContext,
        IEmbeddingService embeddingService,
        UserService userService)
    {
        _dbContext = dbContext;
        _embeddingService = embeddingService;
        _userService = userService;
        _cosineDistanceThreshold = 1.0f;
        _defaultLimit = 25;
    }

    [HttpGet("posts")]
    public async Task<List<PostResponse>> SearchPosts(
        [FromQuery] SearchPostRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        ArgumentException.ThrowIfNullOrEmpty(request.Query, nameof(request.Query));

        var currentUserProfileId = await _userService.GetCurrentUserProfileIdOrEmptyAsync();
        var notNullOrEmptyCurrentUserProfileId = !string.IsNullOrEmpty(currentUserProfileId);

        var queryEmbeddings = await _embeddingService.GenerateEmbedding(request.Query);

        var posts = await _dbContext.Posts
            .AsNoTracking()
            .Where(p => p.Embedding != null && p.Embedding.CosineDistance(queryEmbeddings) < _cosineDistanceThreshold)
            .OrderBy(p => p.Embedding.CosineDistance(queryEmbeddings))
            .Take(request.Limit ?? _defaultLimit)
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
                IsUpvoted =
                    notNullOrEmptyCurrentUserProfileId &&
                    p.Votes.Any(v => v.IsUpvote && v.ProfileId == currentUserProfileId),
                IsDownvoted =
                    notNullOrEmptyCurrentUserProfileId &&
                    p.Votes.Any(v => !v.IsUpvote && v.ProfileId == currentUserProfileId),
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
        return posts;
    }

    [HttpGet("users")]
    public async Task<List<SearchUserResponse>> SearchUsers(
        [FromQuery] SearchUserRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        ArgumentException.ThrowIfNullOrEmpty(request.Query, nameof(request.Query));

        var normalizedQuery = request.Query.ToLower(CultureInfo.CurrentCulture).Trim();

        var users = await _dbContext.Profiles
            .AsNoTracking()
            .Where(p =>
                EF.Functions.ToTsVector("english", p.FirstName + " " + p.LastName)
                    .Matches(EF.Functions.PhraseToTsQuery("english", request.Query)) ||
                p.Email.Contains(normalizedQuery) ||
                (p.Username != null && p.Username.Contains(normalizedQuery)))
            .Take(request.Limit ?? _defaultLimit)
            .Select(p => new SearchUserResponse
            {
                Id = p.Id,
                Email = p.Email,
                Username = p.Username,
                PhoneNumber = p.PhoneNumber,
                FirstName = p.FirstName,
                LastName = p.LastName,
                ProfilePictureUrl = p.ProfilePictureUrl,
                JobTitle = p.JobTitle,
                Bio = p.Bio,
                Rank = EF.Functions.ToTsVector("english", p.FirstName + " " + p.LastName)
                    .Rank(EF.Functions.PhraseToTsQuery("english", normalizedQuery))
            })
            .OrderByDescending(p => p.Rank)
            .ToListAsync(cancellationToken);
        return users;
    }

    [HttpGet("jobs")]
    public async Task<List<JobPostingResponse>> SearchJobs(
        [FromQuery] SearchJobPostsRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        var userProfileId = await _userService.GetCurrentUserProfileIdOrEmptyAsync();

        var queryEmbedding = await _embeddingService.GenerateEmbedding(request.Query);

        var query = _dbContext.JobPostings
            .AsNoTracking()
            .FullyPopulatedJobPostingQuery()
            .AsQueryable();

        if (request.IsRemote)
        {
            query = query.Where(j => j.Area.ToLower().Contains("remote"));
        }
        else if (!string.IsNullOrEmpty(request.Area))
        {
            request.Area = request.Area.Trim().ToLower(CultureInfo.CurrentCulture);
            query = query.Where(j => j.Area.ToLower().Contains(request.Area));
        }

        if (request.MinBudget is >= 0)
        {
            query = query.Where(j => j.Budget >= request.MinBudget.Value);
        }

        if (request.MaxBudget is >= 0 &&
            request.MaxBudget.Value > request.MinBudget.GetValueOrDefault(0))
        {
            query = query.Where(j => j.Budget <= request.MaxBudget.Value);
        }

        query = query
            .Where(j => j.Embedding != null && j.Embedding.CosineDistance(queryEmbedding) < _cosineDistanceThreshold)
            .OrderBy(j => j.Embedding.CosineDistance(queryEmbedding))
            .Take(request.Limit ?? _defaultLimit);


        var jobPosts = await query
            .Select(p => new JobPostingResponse
            {
                IsUpvoted = p.Votes.Any(v => v.IsUpvote && v.ProfileId == userProfileId),
                IsDownvoted = p.Votes.Any(v => !v.IsUpvote && v.ProfileId == userProfileId),
                Title = p.Title,
                Content = p.Content,
                Area = p.Area,
                Budget = p.Budget,
                Language = p.Language,
                Tags = p.JobPostingTags.Select(pt => pt.Tag.SelectTagResponseFromTag()).ToList(),
                Author = p.Author.SelectAuthorResponseFromProfile(),
                CreatedAt = p.CreatedAt,
                LastModified = p.UpdatedAt,
                Id = p.Id,
                Upvotes = p.Votes.Count(v => v.IsUpvote),
                Downvotes = p.Votes.Count(v => !v.IsUpvote),
                Comments = p.Comments.Count,
                IsAppliedFor = p.JobApplications.Any(ja => ja.ApplicantId == userProfileId),
                Medias = p.Medias.Select(m => new MediaObjectResponse
                {
                    Id = m.Id,
                    Name = m.Name,
                    Type = m.Type,
                    Url = $"https://expert-bridge-media.s3.amazonaws.com/{m.Key}"
                }).ToList(),
                RelevanceScore = p.Embedding.CosineDistance(queryEmbedding)
            })
            .ToListAsync(cancellationToken);

        return jobPosts;
    }
}
