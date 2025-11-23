using ExpertBridge.Api.Services;
using ExpertBridge.Contract.Requests.SearchJobPosts;
using ExpertBridge.Contract.Requests.SearchPost;
using ExpertBridge.Contract.Requests.SearchUser;
using ExpertBridge.Contract.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    /// <summary>
    ///     Represents the service responsible for handling search-related operations,
    ///     such as retrieving posts, users, and job postings, based on specific queries.
    /// </summary>
    private readonly SearchService _searchService;

    /// <summary>
    ///     Handles search-related endpoints for posts, users, and jobs.
    ///     Provides an API interface for retrieving data based on search queries
    ///     by utilizing the SearchService.
    /// </summary>
    /// <remarks>
    ///     This controller handles requests from the client side and delegates
    ///     search tasks to the underlying SearchService. It is designed to be
    ///     stateless and supports asynchronous operations.
    ///     All operations under this controller are accessible without authorization.
    /// </remarks>
    public SearchController(SearchService searchService)
    {
        _searchService = searchService;
    }

    /// <summary>
    ///     Retrieves a list of posts that match the specified search criteria.
    ///     Utilizes the search service to query and return posts based on the provided request parameters.
    /// </summary>
    /// <param name="request">The request object containing search criteria such as filters and keywords.</param>
    /// <param name="cancellationToken">The cancellation token to observe for task cancellation requests.</param>
    /// <returns>A list of posts that match the search criteria encapsulated in the response structure.</returns>
    [HttpGet("posts")]
    public async Task<List<PostResponse>> SearchPosts(
        [FromQuery] SearchPostRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return await _searchService.SearchPosts(request, cancellationToken);
    }

    /// <summary>
    ///     Searches for users based on the specified query parameters.
    ///     Retrieves a list of users that match the provided search criteria.
    /// </summary>
    /// <param name="request">The search query parameters for filtering users.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A list of users matching the search criteria.</returns>
    [HttpGet("users")]
    public async Task<List<SearchUserResponse>> SearchUsers(
        [FromQuery] SearchUserRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return await _searchService.SearchUsers(request, cancellationToken);
    }

    /// <summary>
    ///     Searches for job postings based on the specified search criteria provided in the request.
    ///     Delegates the search operation to the underlying SearchService and retrieves matching job postings.
    /// </summary>
    /// <param name="request">
    ///     The request containing search criteria for querying job postings.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to observe while waiting for the task to complete. Optional, defaults to CancellationToken.None.
    /// </param>
    /// <returns>
    ///     A list of job postings that match the search criteria, encapsulated in <see cref="JobPostingResponse" /> objects.
    /// </returns>
    [HttpGet("jobs")]
    public async Task<List<JobPostingResponse>> SearchJobs(
        [FromQuery] SearchJobPostsRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return await _searchService.SearchJobs(request, cancellationToken);
    }
}
