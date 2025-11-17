using ExpertBridge.Api.Services;
using ExpertBridge.Application.Settings;
using ExpertBridge.Contract.Requests.CreatePost;
using ExpertBridge.Contract.Requests.EditPost;
using ExpertBridge.Contract.Requests.PostsCursor;
using ExpertBridge.Contract.Responses;
using ExpertBridge.Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ExpertBridge.Api.Controllers;

/// <summary>
///     Controller for managing posts in the ExpertBridge platform.
///     Handles post-creation, retrieval, voting, editing, and deletion operations.
/// </summary>
/// <remarks>
///     Most endpoints require authentication except for GetById, GetSimilarPosts, GetSuggestedPosts,
///     GetCursorPaginated, and GetAllByProfileId, which allow anonymous access.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize] // Ensure user is authenticated for all actions except GetById and GetSimilarPosts
[ResponseCache(CacheProfileName = CacheProfiles.PersonalizedContent)]
public class PostsController : ControllerBase
{
    private readonly PostService _postService;
    private readonly UserService _userService;

    /// <summary>
    ///     Initializes a new instance of the <see cref="PostsController" /> class.
    /// </summary>
    /// <param name="postService">The service for managing post operations.</param>
    /// <param name="userService">The service for managing user operations.</param>
    public PostsController(
        PostService postService,
        UserService userService)
    {
        _postService = postService;
        _userService = userService;
    }

    /// <summary>
    ///     Create a new post.
    /// </summary>
    /// <param name="request">
    ///     The request containing the post data.
    /// </param>
    /// <returns>
    ///     The created post response.
    /// </returns>
    /// <exception cref="UnauthorizedException">
    ///     Thrown when the user is not authenticated or the user is not authorized to create a post.
    /// </exception>
    /// <exception cref="Microsoft.AspNetCore.Server.Kestrel.Core.BadHttpRequestException">
    ///     Thrown when the request is invalid.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the request is null.
    /// </exception>
    [HttpPost]
    public async Task<ActionResult<PostResponse>> Create([FromBody] CreatePostRequest request)
    {
        // Get user profile - UserService.GetCurrentUserProfileOrThrowAsync handles unauthorized/missing profile
        var authorProfile = await _userService.GetCurrentUserProfileOrThrowAsync();

        var postResponse = await _postService.CreatePostAsync(request, authorProfile);

        // HTTP 201 Created with Location header
        return CreatedAtAction(nameof(GetById), new { postId = postResponse.Id }, postResponse);
    }

    /// <summary>
    ///     Get post by id.
    /// </summary>
    /// <param name="postId">
    ///     The ID of the post to get.
    /// </param>
    /// <returns>
    ///     The post response.
    /// </returns>
    /// <exception cref="PostNotFoundException">
    ///     Thrown when the post with the given ID does not exist.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Thrown when the postId is null or empty.
    /// </exception>
    [AllowAnonymous]
    [HttpGet("{postId}", Name = "GetPostByIdAction")] // Added Name for CreatedAtAction
    public async Task<ActionResult<PostResponse>> GetById([FromRoute] string postId)
    {
        // string? userProfileId = await _userService.GetCurrentUserProfileIdOrEmptyAsync(); // Get this if needed for vote perspective
        // For now, assuming your UserService provides a method like this from previous discussion:
        var user = await _userService.GetCurrentUserPopulatedModelAsync(); // Or your specific method
        var userProfileId = user?.Profile?.Id;


        var postResponse = await _postService.GetPostByIdAsync(postId, userProfileId);

        if (postResponse == null)
        {
            // return post ?? throw new PostNotFoundException($"Post with id={postId} was not found");
            return NotFound(new ProblemDetails
            {
                Title = "Not Found",
                Detail = $"Post with id={postId} was not found.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return postResponse;
    }

    /// <summary>
    ///     Retrieves a list of posts that are similar to the specified post.
    /// </summary>
    /// <param name="postId">The unique identifier of the post to find similar posts for.</param>
    /// <param name="limit">The maximum number of similar posts to return. Defaults to 5 if not specified.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A list of similar posts as <see cref="SimilarPostsResponse" /> objects.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="postId" /> is null or empty.</exception>
    /// <remarks>This endpoint allows anonymous access.</remarks>
    [AllowAnonymous]
    [HttpGet("{postId}/similar")]
    public async Task<List<SimilarPostsResponse>> GetSimilarPosts(
        [FromRoute] string postId,
        [FromQuery] int? limit = 5,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(postId); // Service handles

        var similarPosts = await _postService.GetSimilarPostsAsync(
            postId,
            limit ?? 5, // Default to 5 if not provided
            cancellationToken);

        return similarPosts;
    }

    /// <summary>
    ///     Retrieves a list of suggested posts for the current user based on their profile and interests.
    ///     If the user is not authenticated, returns general suggested posts.
    /// </summary>
    /// <param name="limit">The maximum number of suggested posts to return. Defaults to 5 if not specified.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A list of suggested posts as <see cref="SimilarPostsResponse" /> objects.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the requestUri must be an absolute URI or BaseAddress must be
    ///     set.
    /// </exception>
    /// <exception cref="HttpRequestException">
    ///     Thrown when the request failed due to an underlying issue such as network
    ///     connectivity, DNS failure, server certificate validation, or timeout.
    /// </exception>
    /// <exception cref="TaskCanceledException">
    ///     Thrown when the request failed due to timeout (.NET Core and .NET 5 and later
    ///     only).
    /// </exception>
    /// <exception cref="OperationCanceledException">Thrown when the CancellationToken is canceled.</exception>
    /// <remarks>This endpoint allows anonymous access.</remarks>
    [AllowAnonymous]
    [HttpGet("suggested")]
    public async Task<ActionResult<List<SimilarPostsResponse>>> GetSuggestedPosts(
        [FromQuery] int? limit,
        CancellationToken cancellationToken = default)
    {
        var user = await _userService.GetCurrentUserPopulatedModelAsync();
        var suggestedPosts = await _postService.GetSuggestedPostsAsync(
            user?.Profile,
            limit ?? 5, // Default to 5 if not provided
            cancellationToken);

        return suggestedPosts;
    }

    /// <summary>
    ///     Retrieves a paginated feed of recommended posts using cursor-based pagination.
    ///     Personalizes the feed based on the current user's profile if authenticated.
    /// </summary>
    /// <param name="request">
    ///     The cursor pagination request containing pagination parameters such as cursor position and page
    ///     size.
    /// </param>
    /// <returns>A paginated response containing posts and cursor information for the next page.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the request is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the requestUri must be an absolute URI or BaseAddress must be
    ///     set.
    /// </exception>
    /// <exception cref="HttpRequestException">
    ///     Thrown when the request failed due to an underlying issue such as network
    ///     connectivity, DNS failure, server certificate validation, or timeout.
    /// </exception>
    /// <exception cref="TaskCanceledException">
    ///     Thrown when the request failed due to timeout (.NET Core and .NET 5 and later
    ///     only).
    /// </exception>
    /// <exception cref="Microsoft.EntityFrameworkCore.DbUpdateException">
    ///     Thrown when an error is encountered while saving to
    ///     the database.
    /// </exception>
    /// <exception cref="Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException">
    ///     Thrown when a concurrency violation is
    ///     encountered while saving to the database.
    /// </exception>
    /// <exception cref="OperationCanceledException">Thrown when the CancellationToken is canceled.</exception>
    /// <remarks>
    ///     This endpoint allows anonymous access. Response caching is disabled (NoStore = true) to ensure fresh,
    ///     personalized content is always returned.
    /// </remarks>
    [AllowAnonymous]
    [HttpPost("feed")]
    [ResponseCache(NoStore = true)]
    public async Task<ActionResult<PostsCursorPaginatedResponse>> GetCursorPaginated(
        [FromBody] PostsCursorRequest request)
    {
        var user = await _userService.GetCurrentUserPopulatedModelAsync();

        var posts = await _postService.GetRecommendedPostsOffsetPageAsync(user?.Profile, request);

        return posts;
    }

    /// <summary>
    ///     Retrieves all posts created by a specific profile.
    ///     If the requesting user is authenticated, the posts will include personalized data such as vote status.
    /// </summary>
    /// <param name="profileId">The unique identifier of the profile whose posts should be retrieved.</param>
    /// <returns>A list of posts created by the specified profile as <see cref="PostResponse" /> objects.</returns>
    /// <exception cref="ArgumentException">Thrown when the profileId is null or empty.</exception>
    /// <exception cref="Microsoft.EntityFrameworkCore.DbUpdateException">
    ///     Thrown when an error is encountered while saving to
    ///     the database.
    /// </exception>
    /// <exception cref="Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException">
    ///     Thrown when a concurrency violation is
    ///     encountered while saving to the database.
    /// </exception>
    /// <exception cref="OperationCanceledException">Thrown when the CancellationToken is canceled.</exception>
    /// <remarks>This endpoint allows anonymous access and is accessible via the route /api/profiles/{profileId}/posts.</remarks>
    [AllowAnonymous]
    [HttpGet]
    [Route("/api/profiles/{profileId}/posts")]
    public async Task<ActionResult<List<PostResponse>>> GetAllByProfileId([FromRoute] string profileId)
    {
        var user = await _userService.GetCurrentUserPopulatedModelAsync();
        var requestingUserProfileId = user?.Profile?.Id;

        var posts = await _postService.GetPostsByProfileIdAsync(profileId, requestingUserProfileId);
        return posts;
    }

    /// <summary>
    ///     UpVote a post. If the user has already upvoted the post, it will remove the vote.
    ///     If the user has downvoted the post, it will remove the downvote and add an upvote.
    ///     If the user has not voted on the post, it will add an upvote.
    /// </summary>
    /// <param name="postId">
    ///     The ID of the post to upvote.
    /// </param>
    /// <exception cref="UnauthorizedException">
    ///     Thrown when the user is not authenticated.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Thrown when the postId is null or empty.
    /// </exception>
    /// <exception cref="PostNotFoundException">
    ///     Thrown when the post with the given ID does not exist.
    /// </exception>
    /// <returns>
    ///     The post response with the updated votes count.
    /// </returns>
    [HttpPatch("{postId}/upvote")]
    public async Task<ActionResult<PostResponse>> Upvote([FromRoute] string postId)
    {
        var voterProfile =
            await _userService.GetCurrentUserProfileOrThrowAsync(); // Ensures authenticated user with profile

        var postResponse = await _postService.VotePostAsync(postId, voterProfile, true); // true for upvote
        return postResponse;
    }

    /// <summary>
    ///     DownVote a post. If the user has already downvoted the post, it will remove the vote.
    ///     If the user has upvoted the post, it will remove the upvote and add a downvote.
    ///     If the user has not voted on the post, it will add a downvote.
    /// </summary>
    /// <param name="postId">
    ///     The ID of the post to downvote.
    /// </param>
    /// <returns>
    ///     The post response with the updated votes count.
    /// </returns>
    /// <exception cref="UnauthorizedException">
    ///     Thrown when the user is not authenticated or the user is not authorized to access the post.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Thrown when the postId is null or empty.
    /// </exception>
    /// <exception cref="PostNotFoundException">
    ///     Thrown when the post with the given ID does not exist.
    /// </exception>
    [HttpPatch("{postId}/downvote")]
    public async Task<ActionResult<PostResponse>> Downvote([FromRoute] string postId)
    {
        var voterProfile =
            await _userService.GetCurrentUserProfileOrThrowAsync(); // Ensures authenticated user with profile

        var postResponse = await _postService.VotePostAsync(postId, voterProfile, false); // false for downvote
        return postResponse;
    }

    /// <summary>
    ///     Edit a post. Only the fields that are provided in the request will be updated.
    /// </summary>
    /// <param name="postId">
    ///     The ID of the post to edit.
    /// </param>
    /// <param name="request">
    ///     The request containing the fields to update.
    /// </param>
    /// <exception cref="UnauthorizedException">
    ///     Thrown when the user is not authenticated or not the author of the post.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Thrown when the postId is null or empty.
    /// </exception>
    /// <exception cref="PostNotFoundException">
    ///     Thrown when the post with the given ID does not exist or doesn't belong to the user.
    /// </exception>
    /// <returns>
    ///     The updated post.
    /// </returns>
    [HttpPatch("{postId}")]
    public async Task<ActionResult<PostResponse>> Edit(
        [FromRoute] string postId,
        [FromBody] EditPostRequest request)
    {
        var editorProfile = await _userService.GetCurrentUserProfileOrThrowAsync();

        var postResponse = await _postService.EditPostAsync(postId, request, editorProfile);

        return postResponse;
    }

    /// <summary>
    ///     Delete a post by its ID.
    /// </summary>
    /// <param name="postId">
    ///     The ID of the post to delete.
    /// </param>
    /// <returns>
    ///     No content response.
    /// </returns>
    /// <exception cref="UnauthorizedException">
    ///     Thrown when the user is not authenticated or not the author of the post.
    /// </exception>
    /// <exception cref="PostNotFoundException">
    ///     Thrown when the post with the given ID does not exist or doesn't belong to the user.
    /// </exception>
    [HttpDelete("{postId}")]
    public async Task<IActionResult> Delete([FromRoute] string postId)
    {
        var deleterProfile = await _userService.GetCurrentUserProfileOrThrowAsync();

        try
        {
            await _postService.DeletePostAsync(postId, deleterProfile);
            return NoContent(); // Always 204 for DELETE success (even if resource was already gone)
        }
        catch (ForbiddenAccessException ex)
        {
            // Even for forbidden, you might return 204 to not leak info,
            // or 403 if you want to be explicit. Standard is often 204 for DELETE.
            // However, if _userService.GetCurrentUserProfileOrThrowAsync() throws Unauthorized,
            // that will result in 401 before this.
            // A 403 here means authenticated user, but not permitted for *this specific resource*.
            // For DELETE, many prefer to still return 204 to not reveal existence/non-existence.
            // But if it's a clear "you can't do that" to an owned resource, 403 is also fine.
            // Let's stick to 204 for simplicity and common DELETE idempotency interpretation.
            Log.Warning(ex,
                "Forbidden attempt to delete post {PostId} by user {UserProfileId}",
                postId, deleterProfile.Id);

            return NoContent();
        }
    }
}
