using ExpertBridge.Api.DomainServices;
using ExpertBridge.Api.Settings;
using ExpertBridge.Core.Exceptions;
using ExpertBridge.Core.Requests;
using ExpertBridge.Core.Requests.CreatePost;
using ExpertBridge.Core.Requests.EditPost;
using ExpertBridge.Core.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ExpertBridge.Api.Controllers;

/// <summary>
///     Controller for posts management.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]

[ResponseCache(CacheProfileName = CacheProfiles.PersonalizedContent)]
public class PostsController : ControllerBase
{
    private readonly PostService _postService;
    private readonly UserService _userService;
    private readonly ILogger<PostsController> _logger;

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
    /// <exception cref="BadHttpRequestException">
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
        string? userProfileId = user?.Profile?.Id;


        var postResponse = await _postService.GetPostByIdAsync(postId, userProfileId);

        if (postResponse == null)
        {
            // return post ?? throw new PostNotFoundException($"Post with id={postId} was not found");
            return NotFound(new ProblemDetails { Title = "Not Found", Detail = $"Post with id={postId} was not found.", Status = StatusCodes.Status404NotFound });
        }
        return postResponse;
    }

    /// <summary>
    ///     Get all posts.
    /// </summary>
    /// <returns>
    ///     The list of post responses.
    /// </returns>
    //[AllowAnonymous]
    //[HttpGet]
    //public async Task<ActionResult<List<PostResponse>>> GetAll()
    //{
    //    _logger.LogInformation("User from HTTP Context (GetAllPosts): {FindFirstValue}", HttpContext.User.FindFirstValue(ClaimTypes.Email)); // Keep Serilog if desired

    //    // var user = await _authHelper.GetCurrentUserAsync();
    //    // var userProfileId = user?.Profile?.Id ?? string.Empty;
    //    var user = await _userService.GetCurrentUserPopulatedModelAsync();
    //    string? userProfileId = user?.Profile?.Id;

    //    var posts = await _postService.GetAllPostsAsync(userProfileId);
    //    return posts;
    //}

    [AllowAnonymous]
    [HttpGet]
    [ResponseCache(NoStore = true)]
    public async Task<ActionResult<PostsCursorPaginatedResponse>> GetCursorPaginated(
        [FromQuery] PostsCursorRequest request)
    {
        var user = await _userService.GetCurrentUserPopulatedModelAsync();
        //var posts = await _postService.GetRecommendedPostsAsync(user?.Profile, request);

        var posts = await _postService.GetRecommendedPostsOffsetPageAsync(user?.Profile, request);

        return posts;
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("/api/profiles/{profileId}/posts")]
    public async Task<ActionResult<List<PostResponse>>> GetAllByProfileId([FromRoute] string profileId)
    {
        // ArgumentException.ThrowIfNullOrEmpty(profileId, nameof(profileId)); // Service handles

        // var profile = await _dbContext.Profiles... // Service handles profile existence check

        // string? requestingUserProfileId = await _userService.GetCurrentUserProfileIdOrEmptyAsync(); // Or however you get it
        var user = await _userService.GetCurrentUserPopulatedModelAsync();
        string? requestingUserProfileId = user?.Profile?.Id;

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
    ///    Thrown when the post with the given ID does not exist.
    /// </exception>
    /// <returns>
    ///     The post response with the updated votes count.
    /// </returns>
    [HttpPatch("{postId}/upvote")]
    public async Task<ActionResult<PostResponse>> Upvote([FromRoute] string postId)
    {
        var voterProfile = await _userService.GetCurrentUserProfileOrThrowAsync(); // Ensures authenticated user with profile

        var postResponse = await _postService.VotePostAsync(postId, voterProfile, isUpvoteIntent: true); // true for upvote
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
        var voterProfile = await _userService.GetCurrentUserProfileOrThrowAsync(); // Ensures authenticated user with profile

        var postResponse = await _postService.VotePostAsync(postId, voterProfile, isUpvoteIntent: false); // false for downvote
        return postResponse;
    }

    /// <summary>
    ///     Edit a post. Only the fields that are provided in the request will be updated.
    /// </summary>
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
    ///    Thrown when the post with the given ID does not exist or doesn't belong to the user.
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

    // BEWARE!
    // In an HTTP DELETE, you always want to return 204 no content.
    // No matter what happens. The only exception is if the auth middleware
    // refused the request from the beginning, else you do not return anything
    // other than no content.
    // https://stackoverflow.com/questions/6439416/status-code-when-deleting-a-resource-using-http-delete-for-the-second-time#comment33002038_6440374
}
