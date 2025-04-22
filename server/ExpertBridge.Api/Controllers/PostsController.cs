using System.Security.Claims;
using ExpertBridge.Api.Core;
using ExpertBridge.Api.Core.Entities.Media.PostMedia;
using ExpertBridge.Api.Core.Entities.Posts;
using ExpertBridge.Api.Core.Entities.PostVotes;
using ExpertBridge.Api.Data.DatabaseContexts;
using ExpertBridge.Api.Helpers;
using ExpertBridge.Api.Queries;
using ExpertBridge.Api.Requests.CreatePost;
using ExpertBridge.Api.Requests.EditPost;
using ExpertBridge.Api.Responses;
using ExpertBridge.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ExpertBridge.Api.Controllers;

/// <summary>
///     Controller for posts management.
/// </summary>
/// <param name="dbContext">
///     The database context.
/// </param>
/// <param name="authHelper">
///     The authorization helper.
/// </param>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PostsController(
    ExpertBridgeDbContext dbContext,
    AuthorizationHelper authHelper
    ) : ControllerBase
{
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
    [HttpGet("{postId}")]
    public async Task<PostResponse> GetById([FromRoute] string postId)
    {
        ArgumentException.ThrowIfNullOrEmpty(postId, nameof(postId));
        var user = await authHelper.GetCurrentUserAsync(User);
        var userProfileId = user?.Profile?.Id;

        var post = await dbContext.Posts
            .FullyPopulatedPostQuery(p => p.Id == postId)
            .SelectPostResponseFromFullPost(userProfileId)
            .FirstOrDefaultAsync();

        return post ?? throw new PostNotFoundException($"Post with id={postId} was not found");
    }

    /// <summary>
    ///     Get all posts.
    /// </summary>
    /// <returns>
    ///     The list of post responses.
    /// </returns>
    [AllowAnonymous]
    [HttpGet]
    public async Task<List<PostResponse>> GetAll()
    {
        Log.Information($"User from HTTP Context: {HttpContext.User.FindFirstValue(ClaimTypes.Email)}");

        var user = await authHelper.GetCurrentUserAsync(User);
        var userProfileId = user?.Profile?.Id ?? string.Empty;

        return await dbContext.Posts
            .FullyPopulatedPostQuery()
            .SelectPostResponseFromFullPost(userProfileId)
            .ToListAsync();
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
    public async Task<PostResponse> Create([FromBody] CreatePostRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        var user = await authHelper.GetCurrentUserAsync(User);
        var userProfileId = user?.Profile?.Id ?? string.Empty;

        if (string.IsNullOrEmpty(userProfileId))
        {
            throw new UnauthorizedException();
        }

        if (string.IsNullOrEmpty(request?.Title) || string.IsNullOrEmpty(request?.Content))
        {
            throw new BadHttpRequestException("Title and Content are required");
        }

        var post = new Post
        {
            Title = request.Title,
            Content = request.Content,
            AuthorId = userProfileId
        };

        await dbContext.Posts.AddAsync(post);

        if (request.Media?.Count > 0)
        {
            var postMedia = new List<PostMedia>();
            foreach (var media in request.Media)
            {
                postMedia.Add(new PostMedia
                {
                    Post = post,
                    Name = post.Title,
                    Type = media.Type,
                    Key = media.Key,
                });

            }

            await dbContext.PostMedias.AddRangeAsync(postMedia);
            post.Medias = postMedia;
        }

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            var x = 1;
            throw;
        }

        return post.SelectPostResponseFromFullPost(userProfileId);
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
    public async Task<PostResponse> Upvote([FromRoute] string postId)
    {
        // Get the current user from the HTTP context
        var user = await authHelper.GetCurrentUserAsync(User);
        var userProfileId = user?.Profile?.Id ?? string.Empty;

        // if the user is not authenticated, throw an exception
        if (string.IsNullOrEmpty(userProfileId))
            throw new UnauthorizedException($"Unauthorized Access from User {User}");

        // Check if the postId is valid
        ArgumentException.ThrowIfNullOrEmpty(postId, nameof(postId));

        // Check if the post exists in the database
        var post = await dbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId);
        if (post is null) throw new PostNotFoundException($"Post with id={postId} does not exist!");

        var vote = await dbContext.PostVotes
                .FirstOrDefaultAsync(v => v.PostId == postId && v.ProfileId == userProfileId);

        if (vote is null)
        {
            // If the vote does not exist, create a new one
            var newVote = new PostVote
            {
                Post = post,
                Profile = user.Profile,
                IsUpvote = true,
            };
            await dbContext.PostVotes.AddAsync(newVote);
        }
        else if (!vote.IsUpvote)
        {
            // If the vote exists but is a downvote, update it to an upvote
            vote.IsUpvote = true;
            vote.LastModified = DateTime.UtcNow;
        }
        else
        {
            // If the vote exists and is an upvote, remove it (toggle behavior)
            dbContext.PostVotes.Remove(vote);
        }

        // Save changes to the database
        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            var x = 1;
            throw;
        }

        return await dbContext.Posts
            .FullyPopulatedPostQuery(p => p.Id == postId)
            .SelectPostResponseFromFullPost(userProfileId)
            .FirstAsync();
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
    public async Task<PostResponse> Downvote([FromRoute] string postId)
    {
        // Get the current user from the HTTP context
        var user = await authHelper.GetCurrentUserAsync(User);
        var userProfileId = user?.Profile?.Id ?? string.Empty;

        // if the user is not authenticated, throw an exception
        if (string.IsNullOrEmpty(userProfileId))
            throw new UnauthorizedException($"Unauthorized Access from User {User}");

        // Check if the postId is valid
        ArgumentException.ThrowIfNullOrEmpty(postId, nameof(postId));

        // Check if the post exists in the database
        var post = await dbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId);
        if (post is null) throw new PostNotFoundException($"Post with id={postId} does not exist!");

        var vote = await dbContext.PostVotes
                .FirstOrDefaultAsync(v => v.PostId == postId && v.ProfileId == userProfileId);

        if (vote is null)
        {
            // If the vote does not exist, create a new one
            var newVote = new PostVote
            {
                PostId = postId,
                ProfileId = userProfileId,
                IsUpvote = false,
            };
            await dbContext.PostVotes.AddAsync(newVote);
        }
        else if (vote.IsUpvote)
        {
            // If the vote exists but is a upvote, update it to a downvote
            vote.IsUpvote = false;
            vote.LastModified = DateTime.UtcNow;
        }
        else
        {
            // If the vote exists and is a downvote, remove it (toggle behavior)
            dbContext.PostVotes.Remove(vote);
        }

        // Save changes to the database
        await dbContext.SaveChangesAsync();

        return await dbContext.Posts
            .FullyPopulatedPostQuery(p => p.Id == postId)
            .SelectPostResponseFromFullPost(userProfileId)
            .FirstAsync();
    }

    /// <summary>
    ///     Edit a post. Only the fields that are provided in the request will be updated.
    /// </summary>
    /// <param name="editPostRequest">
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
    [HttpPatch("")]
    public async Task<PostResponse> Edit([FromBody] EditPostRequest editPostRequest)
    {
        // Get the current user from the HTTP context
        var user = await authHelper.GetCurrentUserAsync(User);
        if (user is null) throw new UnauthorizedException($"Unauthorized Access from User {User}");
        var userProfileId = user.Profile.Id;
        if (string.IsNullOrEmpty(userProfileId))
            throw new UnauthorizedException($"Unauthorized Access from User {User}");

        // Check if the postId is valid
        ArgumentNullException.ThrowIfNull(editPostRequest, nameof(editPostRequest));
        ArgumentException.ThrowIfNullOrEmpty(editPostRequest.Id, nameof(editPostRequest.Id));
        var post = await dbContext.Posts.FirstOrDefaultAsync(p => p.Id == editPostRequest.Id && p.AuthorId == userProfileId);
        if (post is null)
            throw new PostNotFoundException($"Post with id={editPostRequest.Id} does not exist for user with id{user.Id}!");

        // Update the post with the new values and save changes to the database.
        if (!string.IsNullOrEmpty(editPostRequest.Title))
            post.Title = editPostRequest.Title;
        if (!string.IsNullOrEmpty(editPostRequest.Content))
            post.Content = editPostRequest.Content;
        post.LastModified = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();

        // Return the updated post
        return await dbContext.Posts
            .FullyPopulatedPostQuery(p => p.Id == editPostRequest.Id)
            .SelectPostResponseFromFullPost(userProfileId)
            .FirstAsync();
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
        ArgumentException.ThrowIfNullOrEmpty(postId, nameof(postId));
        // Check if the user exists in the database
        var user = await authHelper.GetCurrentUserAsync(User);
        if (user is null) throw new UnauthorizedException($"Unauthorized Access from User {User}");
        var userProfileId = user.Profile.Id;
        if (string.IsNullOrEmpty(userProfileId))
            throw new UnauthorizedException($"Unauthorized Access from User {User}");

        // Check if the post exists in the database
        var post = await dbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId && p.AuthorId == userProfileId);
        if (post is null) throw new PostNotFoundException($"Post with id={postId} does not exist!");

        // Remove the post from the database
        dbContext.Posts.Remove(post);
        await dbContext.SaveChangesAsync();
        return NoContent();
    }
}
