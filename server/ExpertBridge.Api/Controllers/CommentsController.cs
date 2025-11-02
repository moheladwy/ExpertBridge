using ExpertBridge.Application.DomainServices;
using ExpertBridge.Application.Settings;
using ExpertBridge.Contract.Requests.CreateComment;
using ExpertBridge.Contract.Requests.EditComment;
using ExpertBridge.Contract.Responses;
using ExpertBridge.Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ExpertBridge.Api.Controllers;

/// <summary>
///     The CommentsController handles operations related to managing comments within the system.
/// </summary>
/// <remarks>
///     This controller provides endpoints for creating, retrieving, editing, upvoting,
///     downvoting, and deleting comments. It also includes methods for retrieving
///     comments associated with specific posts, job postings, or user profiles.
/// </remarks>
/// <example>
///     The CommentsController uses routes and attributes to define API endpoints
///     for handling comment-related functionalities within the application.
/// </example>
[ApiController]
[Authorize]
[Route("api/[controller]")]
[ResponseCache(CacheProfileName = CacheProfiles.PersonalizedContent, Duration = 60)]
public class CommentsController : ControllerBase
{
    /// <summary>
    ///     An instance of the CommentService class used to manage and handle comment-related operations within the
    ///     CommentsController.
    /// </summary>
    private readonly CommentService _commentService;

    /// <summary>
    ///     An instance of the UserService class utilized to manage and handle user-related operations within the
    ///     CommentsController.
    /// </summary>
    private readonly UserService _userService;

    /// <summary>
    ///     The CommentsController handles operations related to user comments.
    ///     Provides endpoints for managing and retrieving comments.
    /// </summary>
    /// <remarks>
    ///     This controller is secured using the [Authorize] attribute, meaning access is restricted to authenticated users
    ///     only.
    ///     Uses response caching with personalized content for improved performance.
    /// </remarks>
    public CommentsController(
        UserService userService,
        CommentService commentService)
    {
        _userService = userService;
        _commentService = commentService;
    }

    /// <summary>
    ///     Creates a new comment based on the provided request data.
    /// </summary>
    /// <remarks>
    ///     This action handles the creation of a comment by accepting data provided in the request body,
    ///     associating it with the current authenticated user's profile, and persisting it to storage.
    ///     Additionally, it returns the created comment with its assigned ID and location URL.
    /// </remarks>
    /// <param name="request">The request object containing the data required to create a comment.</param>
    /// <returns>
    ///     Returns a 201 Created response containing the newly created comment within the response body
    ///     and a Location header pointing to the resource's URL.
    /// </returns>
    [HttpPost]
    public async Task<ActionResult<CommentResponse>> Create([FromBody] CreateCommentRequest request)
    {
        // Use UserService to get the profile, it handles unauthorized/not found internally
        var authorProfile = await _userService.GetCurrentUserProfileOrThrowAsync();

        var comment = await _commentService.CreateCommentAsync(request, authorProfile);

        // HTTP 201 Created with Location header pointing to the new resource
        return CreatedAtAction(nameof(Get), new { commentId = comment.Id }, comment);
    }

    /// <summary>
    ///     Retrieves a comment by its identifier.
    /// </summary>
    /// <param name="commentId">
    ///     The unique identifier of the comment to be retrieved.
    /// </param>
    /// <returns>
    ///     A <see cref="CommentResponse" /> containing the details of the requested comment.
    ///     Throws a <see cref="CommentNotFoundException" /> if no comment is found with the provided identifier.
    /// </returns>
    [HttpGet("{commentId}", Name = "GetCommentById")] // Ensure Name is present for CreatedAtAction
    public async Task<CommentResponse> Get([FromRoute] string commentId)
    {
        var userProfileId = await _userService.GetCurrentUserProfileIdOrEmptyAsync();
        var comment = await _commentService.GetCommentAsync(commentId, userProfileId);

        if (comment == null)
        {
            throw new CommentNotFoundException($"No comment was found with id={commentId}.");
        }

        return comment;
    }

    /// <summary>
    ///     Retrieves all comments associated with a specific post.
    ///     Supports fetching comments for a given post ID.
    /// </summary>
    /// <param name="postId">
    ///     The unique identifier of the post for which the comments are to be retrieved.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation, containing a list of <see cref="CommentResponse" /> objects
    ///     representing the comments for the specified post.
    /// </returns>
    [Route("/api/posts/{postId}/comments")]
    [AllowAnonymous]
    [HttpGet] // api/posts/<postId>/comments
    public async Task<List<CommentResponse>> GetAllByPostId([FromRoute] string postId)
    {
        var userProfileId = await _userService.GetCurrentUserProfileIdOrEmptyAsync();
        var comments = await _commentService
            .GetCommentsByPostAsync(postId, userProfileId);

        return comments;
    }

    /// <summary>
    ///     Retrieves all comments associated with a specific job posting.
    /// </summary>
    /// <param name="jobPostingId">
    ///     The unique identifier for the job posting for which the comments are to be retrieved.
    /// </param>
    /// <returns>
    ///     A list of <see cref="CommentResponse" /> objects representing the comments for the specified job posting.
    /// </returns>
    [Route("/api/jobPostings/{jobPostingId}/comments")]
    [AllowAnonymous]
    [HttpGet] // api/jobPostings/<jobPostingId>/comments
    public async Task<List<CommentResponse>> GetAllByJobPostingId([FromRoute] string jobPostingId)
    {
        var userProfileId = await _userService.GetCurrentUserProfileIdOrEmptyAsync();
        var comments = await _commentService
            .GetCommentsByJobAsync(jobPostingId, userProfileId);

        return comments;
    }

    /// <summary>
    ///     Retrieves all comments associated with a specific profile ID.
    /// </summary>
    /// <param name="profileId">The identifier of the profile for which the comments are retrieved.</param>
    /// <returns>
    ///     A list of comments represented as <see cref="CommentResponse" /> objects associated with the specified
    ///     profile.
    /// </returns>
    [Route("/api/profiles/{profileId}/comments")]
    [HttpGet]
    [AllowAnonymous]
    public async Task<List<CommentResponse>> GetAllByProfileId([FromRoute] string profileId)
    {
        // currentMaybeUserProfileId is passed for consistency, but for "comments by profile X",
        // the perspective for IsUpvoted/IsDownvoted is often the *requesting user*, not profileId.
        // If profileId is meant to be the perspective, then pass profileId to SelectCommentResponseFromFullComment.
        // Usually, it's the *current authenticated user's* perspective.
        var user = await _userService.GetCurrentUserPopulatedModelAsync();
        var requestingUserProfileId = user?.Profile.Id;

        var comments = await _commentService.GetCommentsByProfileAsync(profileId, requestingUserProfileId);

        return comments;
    }

    /// <summary>
    ///     Handles the action of upvoting a comment identified by its ID.
    ///     Updates the comment's upvote count and returns the updated comment details.
    /// </summary>
    /// <param name="commentId">The unique identifier of the comment to be upvoted.</param>
    /// <returns>A <see cref="CommentResponse" /> containing the details of the updated comment.</returns>
    [HttpPatch("{commentId}/upvote")]
    public async Task<CommentResponse> Upvote([FromRoute] string commentId)
    {
        var voterProfile = await _userService.GetCurrentUserProfileOrThrowAsync(); // Throws if not authorized
        var updatedComment = await _commentService.VoteCommentAsync(commentId, voterProfile, true);

        return updatedComment;
    }

    /// <summary>
    ///     Downvotes a specific comment by its ID.
    ///     Allows an authenticated user to cast a downvote on the specified comment.
    /// </summary>
    /// <param name="commentId">
    ///     The unique identifier of the comment to downvote.
    /// </param>
    /// <returns>
    ///     A <see cref="CommentResponse" /> object representing the updated state of the comment.
    /// </returns>
    [HttpPatch("{commentId}/downvote")]
    public async Task<CommentResponse> Downvote([FromRoute] string commentId)
    {
        var voterProfile = await _userService.GetCurrentUserProfileOrThrowAsync(); // Throws if not authorized
        var updatedComment = await _commentService.VoteCommentAsync(commentId, voterProfile, false);

        return updatedComment;
    }

    /// <summary>
    ///     Edits an existing comment identified by its ID.
    ///     Allows the user to update the content of a specific comment.
    /// </summary>
    /// <param name="commentId">The unique identifier of the comment to be edited.</param>
    /// <param name="request">The request object containing the updated comment details.</param>
    /// <returns>A <see cref="CommentResponse" /> instance representing the updated comment.</returns>
    /// <exception cref="CommentNotFoundException">
    ///     Thrown when the specified comment is not found or the result of the edit process is uncertain.
    /// </exception>
    [HttpPatch("{commentId}")]
    public async Task<CommentResponse> Edit([FromRoute] string commentId, [FromBody] EditCommentRequest request)
    {
        var editorProfile = await _userService.GetCurrentUserProfileOrThrowAsync();

        // Check if the comment exists and belongs to the user
        var updatedComment = await _commentService.EditCommentAsync(commentId, request, editorProfile);

        if (updatedComment == null)
        {
            // This could happen if the comment was deleted between fetch and update, or if edit logic returns null for no changes.
            // The service throws CommentNotFound or ForbiddenAccess, so this path is less likely.
            throw new CommentNotFoundException($"Error: Comment {commentId} edit process result unclear.");
        }

        return updatedComment;
    }

    /// <summary>
    ///     Deletes a specific comment based on the provided comment identifier.
    /// </summary>
    /// <param name="commentId">The unique identifier of the comment to be deleted.</param>
    /// <returns>
    ///     An HTTP 204 No Content status if the deletion is successful or if the resource does not exist.
    ///     This status is also returned in cases of forbidden access to adhere to DELETE idempotency practices.
    /// </returns>
    [HttpDelete("{commentId}")]
    public async Task<IActionResult> Delete([FromRoute] string commentId)
    {
        var deleterProfile = await _userService.GetCurrentUserProfileOrThrowAsync();

        try
        {
            await _commentService.DeleteCommentAsync(commentId, deleterProfile);
            return NoContent(); // Always 204 for DELETE success (even if resource was already gone)
        }
        catch (ForbiddenAccessException ex)
        {
            // BEWARE!
            // In an HTTP DELETE, you always want to return 204 no content.
            // No matter what happens. The only exception is if the auth middleware
            // refused the request from the beginning, else you do not return anything
            // other than no content.
            // https://stackoverflow.com/questions/6439416/status-code-when-deleting-a-resource-using-http-delete-for-the-second-time#comment33002038_6440374

            // Even for forbidden, you might return 204 to not leak info,
            // or 403 if you want to be explicit. Standard is often 204 for DELETE.
            // However, if _userService.GetCurrentUserProfileOrThrowAsync() throws Unauthorized,
            // that will result in 401 before this.
            // A 403 here means authenticated user, but not permitted for *this specific resource*.
            // For DELETE, many prefer to still return 204 to not reveal existence/non-existence.
            // But if it's a clear "you can't do that" to an owned resource, 403 is also fine.
            // Let's stick to 204 for simplicity and common DELETE idempotency interpretation.
            Log.Warning(ex,
                "Forbidden attempt to delete comment {CommentId} by user {UserProfileId}",
                commentId, deleterProfile.Id);

            return NoContent();
        }
    }
}
