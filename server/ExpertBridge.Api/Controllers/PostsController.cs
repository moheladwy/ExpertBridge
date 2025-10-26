using ExpertBridge.Application.DomainServices;
using ExpertBridge.Application.Settings;
using ExpertBridge.Core.Exceptions;
using ExpertBridge.Core.Requests;
using ExpertBridge.Core.Requests.CreatePost;
using ExpertBridge.Core.Requests.EditPost;
using ExpertBridge.Core.Requests.PostsCursor;
using ExpertBridge.Core.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ExpertBridge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Ensure user is authenticated for all actions except GetById and GetSimilarPosts
[ResponseCache(CacheProfileName = CacheProfiles.PersonalizedContent)]
public class PostsController : ControllerBase
{
    private readonly ILogger<PostsController> _logger;
    private readonly PostService _postService;
    private readonly UserService _userService;

    public PostsController(
        PostService postService,
        UserService userService,
        ILogger<PostsController> logger)
    {
        _postService = postService;
        _userService = userService;
        _logger = logger;
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
        ArgumentException.ThrowIfNullOrEmpty(postId, nameof(postId)); // Service handles

        var similarPosts = await _postService.GetSimilarPostsAsync(
            postId,
            limit ?? 5, // Default to 5 if not provided
            cancellationToken);

        return similarPosts;
    }

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
