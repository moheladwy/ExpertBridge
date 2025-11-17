// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using ExpertBridge.Application.EmbeddingService;
using ExpertBridge.Contract.Queries;
using ExpertBridge.Contract.Requests.SearchJobPosts;
using ExpertBridge.Contract.Requests.SearchPost;
using ExpertBridge.Contract.Requests.SearchUser;
using ExpertBridge.Contract.Responses;
using ExpertBridge.Data.DatabaseContexts;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;

namespace ExpertBridge.Api.Services;

/// <summary>
///     Provides search functionality to retrieve posts based on query embeddings and cosine distance similarity.
/// </summary>
public sealed class SearchService
{
    /// <summary>
    ///     The threshold value used to filter posts based on the cosine distance
    ///     between the query embedding and the post embedding. Posts with cosine distances
    ///     greater than this threshold are excluded from the search results.
    /// </summary>
    private readonly float _cosineDistanceThreshold;

    /// <summary>
    ///     Represents the database context used to interact with the underlying data store,
    ///     enabling data access and CRUD operations for entities within the application.
    /// </summary>
    private readonly ExpertBridgeDbContext _dbContext;

    /// <summary>
    ///     The default limit for the number of posts returned in search results
    ///     when no limit is explicitly specified in the search request.
    /// </summary>
    private readonly int _defaultLimit;

    /// <summary>
    ///     The service used to generate query embeddings for text, enabling the comparison
    ///     and retrieval of posts based on vector similarity calculations.
    /// </summary>
    private readonly IEmbeddingService _embeddingService;

    /// <summary>
    ///     Represents the logging mechanism used within the <see cref="SearchService" />
    ///     to capture and log informational, warning, and error messages during
    ///     the execution of search-related operations.
    /// </summary>
    private readonly ILogger<SearchService> _logger;

    /// <summary>
    ///     Validator for the <see cref="SearchJobPostsRequest" /> object, ensuring that
    ///     the input data meets validation criteria for searching job postings.
    /// </summary>
    /// <remarks>
    ///     This validator checks the integrity and validity of the search parameters,
    ///     such as query, budget range, and other optional fields, to ensure they
    ///     adhere to the expected format and constraints.
    /// </remarks>
    private readonly IValidator<SearchJobPostsRequest> _searchJobPostsRequestValidator;

    /// <summary>
    ///     Validator for the <see cref="SearchPostRequest" />, ensuring that the request
    ///     meets the required validation criteria before processing the search operation.
    /// </summary>
    private readonly IValidator<SearchPostRequest> _searchPostRequestValidator;

    /// <summary>
    ///     Validator for the <see cref="SearchUserRequest" />. Ensures that incoming requests
    ///     conform to the expected structure, constraints, and required fields
    ///     when searching for users based on semantic similarity.
    /// </summary>
    private readonly IValidator<SearchUserRequest> _searchUserRequestValidator;

    /// <summary>
    ///     Provides access to user-related operations and data retrieval, enabling functionalities
    ///     such as obtaining the current user's profile information or updating user details.
    ///     Used within the search service to incorporate user-specific logic during operations.
    /// </summary>
    private readonly UserService _userService;

    /// <summary>
    ///     Provides functionalities for managing and handling search-related services in the application.
    /// </summary>
    /// <remarks>
    ///     This service handles processing search requests, validating the requests, and interacting with the database and
    ///     other
    ///     services to retrieve or process search data.
    /// </remarks>
    public SearchService(
        ExpertBridgeDbContext dbContext,
        IEmbeddingService embeddingService,
        UserService userService,
        ILogger<SearchService> logger,
        IValidator<SearchPostRequest> searchPostRequestValidator,
        IValidator<SearchUserRequest> searchUserRequestValidator,
        IValidator<SearchJobPostsRequest> searchJobPostsRequestValidator)
    {
        _dbContext = dbContext;
        _embeddingService = embeddingService;
        _userService = userService;
        _logger = logger;
        _searchPostRequestValidator = searchPostRequestValidator;
        _searchUserRequestValidator = searchUserRequestValidator;
        _searchJobPostsRequestValidator = searchJobPostsRequestValidator;
        _cosineDistanceThreshold = 1.0f;
        _defaultLimit = 50;
    }

    /// <summary>
    ///     Searches for posts based on the provided request parameters.
    /// </summary>
    /// <param name="request">The request object containing search parameters, including query and optional filters or limits.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation if needed.</param>
    /// <returns>A list of <see cref="PostResponse" /> containing the search results that match the specified criteria.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the provided <paramref name="request" /> is null.</exception>
    /// <exception cref="ValidationException">Thrown when the search request fails validation.</exception>
    public async Task<List<PostResponse>> SearchPosts(
        SearchPostRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        _logger.LogInformation("Searching for posts");

        var validationResult = await _searchPostRequestValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogError("SearchPostRequest validation failed: {Errors}", validationResult.Errors);
            throw new ValidationException(validationResult.Errors);
        }

        var currentUserProfileId = await _userService.GetCurrentUserProfileIdOrEmptyAsync();

        var queryEmbeddings = await _embeddingService.GenerateEmbedding(request.Query);

        var posts = await _dbContext.Posts
            .AsNoTracking()
            .Where(p => p.Embedding != null && p.Embedding.CosineDistance(queryEmbeddings) < _cosineDistanceThreshold)
            .OrderBy(p => p.Embedding.CosineDistance(queryEmbeddings))
            .Take(request.Limit ?? _defaultLimit)
            .SelectPostResponseFromFullPost(currentUserProfileId)
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Search for posts completed successfully with {Count} results", posts.Count);
        return posts;
    }

    /// <summary>
    ///     Searches for users based on the provided query and returns a list of matching user profiles.
    /// </summary>
    /// <param name="request">
    ///     The search request containing the query and optional limit for the number of returned profiles.
    /// </param>
    /// <param name="cancellationToken">
    ///     Optional cancellation token to cancel the operation.
    /// </param>
    /// <returns>
    ///     A list of user profiles that match the specified search criteria.
    /// </returns>
    /// <exception cref="ValidationException">
    ///     Thrown when the search request fails validation.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the provided request is null.
    /// </exception>
    public async Task<List<SearchUserResponse>> SearchUsers(
        SearchUserRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        _logger.LogInformation("Searching for users");

        var validationResult = await _searchUserRequestValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogError("SearchUserRequest validation failed: {Errors}", validationResult.Errors);
            throw new ValidationException(validationResult.Errors);
        }

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

        _logger.LogInformation("Search for users completed successfully with {Count} results", users.Count);
        return users;
    }

    /// <summary>
    ///     Searches for job postings based on the provided search criteria.
    /// </summary>
    /// <param name="request">
    ///     The request containing the search criteria such as query, area, budget constraints, and other job-related filters.
    /// </param>
    /// <param name="cancellationToken">
    ///     An optional token to monitor for cancellation requests during the execution of the search.
    /// </param>
    /// <returns>
    ///     A list of job postings matching the search criteria, represented as <see cref="JobPostingResponse" /> objects.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the <paramref name="request" /> is null.
    /// </exception>
    /// <exception cref="ValidationException">
    ///     Thrown when the <paramref name="request" /> validation fails.
    /// </exception>
    public async Task<List<JobPostingResponse>> SearchJobs(
        SearchJobPostsRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        _logger.LogInformation("Searching for job postings");

        var validationResult = await _searchJobPostsRequestValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogError("SearchJobPostsRequest validation failed: {Errors}", validationResult.Errors);
            throw new ValidationException(validationResult.Errors);
        }

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

        var jobPosts = await query
            .Where(j => j.Embedding != null && j.Embedding.CosineDistance(queryEmbedding) < _cosineDistanceThreshold)
            .OrderBy(j => j.Embedding.CosineDistance(queryEmbedding))
            .Take(request.Limit ?? _defaultLimit)
            .SelectJopPostingResponseFromFullJobPosting(userProfileId)
            .ToListAsync(cancellationToken);

        return jobPosts;
    }
}
