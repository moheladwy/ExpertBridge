using ExpertBridge.Application.DomainServices;
using ExpertBridge.Application.Settings;
using ExpertBridge.Core.Exceptions;
using ExpertBridge.Core.Requests.CreateComment;
using ExpertBridge.Core.Requests.EditComment;
using ExpertBridge.Core.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ExpertBridge.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
[ResponseCache(CacheProfileName = CacheProfiles.PersonalizedContent, Duration = 60)]
public class CommentsController : ControllerBase
{
    private readonly UserService _userService;
    private readonly CommentService _commentService;

    public CommentsController(
        UserService userService,
        CommentService commentService)
    {
        _userService = userService;
        _commentService = commentService;
    }

    [HttpPost]
    public async Task<ActionResult<CommentResponse>> Create([FromBody] CreateCommentRequest request)
    {
        // Use UserService to get the profile, it handles unauthorized/not found internally
        var authorProfile = await _userService.GetCurrentUserProfileOrThrowAsync();

        var comment = await _commentService.CreateCommentAsync(request, authorProfile);

        // HTTP 201 Created with Location header pointing to the new resource
        return CreatedAtAction(nameof(Get), new { commentId = comment.Id }, comment);
    }

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

    // CONSIDER!
    // Why not use query params instead of this confusing routing
    // currently used?
    // Something like: /api/comments?postId=aabb33cc

    // THINK!
    // Will migrating to query params make it that more than
    // one endpoint match a certain request? Hmmmm...
    // Ex. /api/comments?postId=aabb33cc /api/comments?userId=bbffcc11

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

    [Route("/api/profiles/{profileId}/[controller]")]
    [HttpGet]
    [AllowAnonymous]
    public async Task<List<CommentResponse>> GetAllByProfileId([FromRoute] string profileId)
    {

        // currentMaybeUserProfileId is passed for consistency, but for "comments by profile X",
        // the perspective for IsUpvoted/IsDownvoted is often the *requesting user*, not profileId.
        // If profileId is meant to be the perspective, then pass profileId to SelectCommentResponseFromFullComment.
        // Usually, it's the *current authenticated user's* perspective.
        var user = await _userService.GetCurrentUserPopulatedModelAsync();
        string? requestingUserProfileId = user?.Profile?.Id;

        var comments = await _commentService.GetCommentsByProfileAsync(profileId, requestingUserProfileId);

        return comments;
    }

    [HttpPatch("{commentId}/upvote")]
    public async Task<CommentResponse> Upvote([FromRoute] string commentId)
    {
        var voterProfile = await _userService.GetCurrentUserProfileOrThrowAsync(); // Throws if not authorized
        var updatedComment = await _commentService.VoteCommentAsync(commentId, voterProfile, isUpvoteIntent: true);

        return updatedComment;
    }

    [HttpPatch("{commentId}/downvote")]
    public async Task<CommentResponse> Downvote([FromRoute] string commentId)
    {
        var voterProfile = await _userService.GetCurrentUserProfileOrThrowAsync(); // Throws if not authorized
        var updatedComment = await _commentService.VoteCommentAsync(commentId, voterProfile, isUpvoteIntent: false);

        return updatedComment;
    }

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

        // BEWARE!
        // In an HTTP DELETE, you always want to return 204 no content.
        // No matter what happens. The only exception is if the auth middleware
        // refused the request from the beginning, else you do not return anything
        // other than no content.
        // https://stackoverflow.com/questions/6439416/status-code-when-deleting-a-resource-using-http-delete-for-the-second-time#comment33002038_6440374
    }
}
