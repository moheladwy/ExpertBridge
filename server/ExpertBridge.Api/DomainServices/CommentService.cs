// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Models.IPC;
using ExpertBridge.Core.Entities.Comments;
using ExpertBridge.Core.Entities.CommentVotes;
using ExpertBridge.Core.Entities.Media.CommentMedia;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Exceptions;
using ExpertBridge.Core.Queries;
using ExpertBridge.Core.Requests;
using ExpertBridge.Core.Requests.CreateComment;
using ExpertBridge.Core.Requests.EditComment;
using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using ExpertBridge.Notifications;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Threading.Channels;

namespace ExpertBridge.Api.DomainServices
{
    public class CommentService
    {
        private readonly ExpertBridgeDbContext _dbContext;
        private readonly MediaAttachmentService _mediaService;
        private readonly NotificationFacade _notificationFacade;
        private readonly ILogger<CommentService> _logger;
        private readonly ChannelWriter<DetectInappropriateCommentMessage> _inappropriateCommentChannel;

        public CommentService(
            ExpertBridgeDbContext dbContext,
            MediaAttachmentService mediaService,
            NotificationFacade notificationFacade,
            Channel<DetectInappropriateCommentMessage> inappropriateCommentChannel,
            ILogger<CommentService> logger)
        {
            _dbContext = dbContext;
            _mediaService = mediaService;
            _notificationFacade = notificationFacade;
            _logger = logger;
            _inappropriateCommentChannel = inappropriateCommentChannel.Writer;
        }

        public async Task<CommentResponse> CreateCommentAsync(CreateCommentRequest request, Profile authorProfile)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentException.ThrowIfNullOrEmpty(request.Content, nameof(request.Content));

            var post = await _dbContext.Posts
                .FirstOrDefaultAsync(p => p.Id == request.PostId);

            if (post == null)
            {
                // Consider throwing a more specific BadHttpRequestException or similar if PostId is invalid
                throw new PostNotFoundException($"Post with id={request.PostId} was not found for comment creation.");
            }

            Comment? parentComment = null;
            if (!string.IsNullOrEmpty(request.ParentCommentId))
            {
                parentComment = await _dbContext.Comments
                    .FirstOrDefaultAsync(c => c.Id == request.ParentCommentId);
                if (parentComment == null)
                {
                    throw new CommentNotFoundException($"Parent comment with id={request.ParentCommentId} was not found.");
                }
            }

            var comment = new Comment
            {
                AuthorId = authorProfile.Id,
                Author = authorProfile,
                Content = request.Content.Trim(),
                ParentCommentId = request.ParentCommentId, // Can be null
                ParentComment = parentComment, // Can be null
                Post = post,
                PostId = post.Id,
            };

            await _dbContext.Comments.AddAsync(comment);

            if (request.Media?.Count > 0)
            {
                CommentMedia createCommentMediaFunc(MediaObjectRequest mediaReq, Comment c) => new CommentMedia
                {
                    Comment = c,
                    Name = SanitizeMediaName(c.Content), // Use a helper for safety
                    Type = mediaReq.Type,
                    Key = mediaReq.Key,
                };

                comment.Medias = await _mediaService.ProcessAndAttachMediaAsync(
                    request.Media,
                    comment,
                    createCommentMediaFunc,
                    _dbContext
                );
            }

            await _dbContext.SaveChangesAsync(); // Single save point for comment, media, grants, notifications

            // Post-save actions
            await _inappropriateCommentChannel.WriteAsync(new DetectInappropriateCommentMessage
            {
                CommentId = comment.Id, // Id is now populated
                Content = comment.Content,
                AuthorId = comment.AuthorId,
            });

            // Actual dispatch of notifications
            await _notificationFacade.NotifyNewCommentAsync(comment);

            // Re-fetch to ensure all navigations are loaded for the response,
            // especially if Author, Medias, Votes etc. are needed by SelectCommentResponseFromFullComment
            // and weren't explicitly loaded or fixed up on the 'comment' instance.
            // Using FirstOrDefaultAsync as Id is unique.
            //var createdCommentWithIncludes = await _dbContext.Comments
            //    .FullyPopulatedCommentQuery(c => c.Id == comment.Id)
            //    .FirstOrDefaultAsync();

            //if (createdCommentWithIncludes == null)
            //{
            //    // This should not happen if SaveChangesAsync() was successful and Id is correct.
            //    // Log error.
            //    throw new InvalidOperationException(
            //        "Failed to retrieve the comment after creation, indicating a data consistency issue.");
            //}

            return comment.SelectCommentResponseFromFullComment(authorProfile.Id);
        }

        public async Task<CommentResponse?> GetCommentAsync(string commentId, string? userProfileId)
        {
            ArgumentException.ThrowIfNullOrEmpty(commentId, nameof(commentId));

            var comment = await _dbContext.Comments
                .FullyPopulatedCommentQuery(c => c.Id == commentId) // Uses your existing query extension
                .SelectCommentResponseFromFullComment(userProfileId)
                .FirstOrDefaultAsync();

            return comment;
        }

        public async Task<List<CommentResponse>> GetCommentsByPostAsync(string postId, string? userProfileId)
        {
            ArgumentException.ThrowIfNullOrEmpty(postId, nameof(postId));

            // Check if post exists - this is good practice to ensure valid foreign key
            var postExists = await _dbContext.Posts.AnyAsync(p => p.Id == postId);
            if (!postExists)
            {
                throw new PostNotFoundException($"Post with id={postId} was not found for retrieving comments.");
            }

            var commentEntities = await _dbContext.Comments
                .FullyPopulatedCommentQuery(c => c.PostId == postId)
                .SelectCommentResponseFromFullComment(userProfileId)
                .ToListAsync();

            return commentEntities;
        }

        public async Task<List<CommentResponse>> GetCommentsByProfileAsync(string profileId, string? userProfileId)
        {
            ArgumentException.ThrowIfNullOrEmpty(profileId, nameof(profileId));

            // Check if profile exists
            var profileExists = await _dbContext.Profiles.AnyAsync(p => p.Id == profileId);
            if (!profileExists)
            {
                throw new UserNotFoundException($"Profile with id={profileId} was not found for retrieving comments.");
            }

            // currentMaybeUserProfileId is passed for consistency, but for "comments by profile X",
            // the perspective for IsUpvoted/IsDownvoted is often the *requesting user*, not profileId.
            // If profileId is meant to be the perspective, then pass profileId to SelectCommentResponseFromFullComment.
            // Usually, it's the *current authenticated user's* perspective.
            var comments = await _dbContext.Comments
                .FullyPopulatedCommentQuery(c => c.AuthorId == profileId)
                .SelectCommentResponseFromFullComment(userProfileId)
                .ToListAsync();

            return comments;
        }

        public async Task<CommentResponse?> EditCommentAsync(string commentId, EditCommentRequest request, Profile editorProfile)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));
            ArgumentException.ThrowIfNullOrEmpty(commentId, nameof(commentId));
            ArgumentNullException.ThrowIfNull(editorProfile, nameof(editorProfile));

            var comment = await _dbContext.Comments
                // Include author to ensure we can map to CommentResponse, or FullyPopulatedCommentQuery does it.
                // .Include(c => c.Author) 
                // .Include(c => c.Votes)
                // .Include(c => c.Medias)
                // For edit, we might only need the basic comment if not re-fetching with FullyPopulated later.
                // Let's fetch with enough data or plan to re-fetch.
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null)
            {
                throw new CommentNotFoundException(
                    $"Comment with id={commentId} was not found for editing.");
            }

            if (comment.AuthorId != editorProfile.Id)
            {
                // Log this attempt
                _logger.LogWarning("User {EditorProfileId} attempted to edit comment {CommentId} owned by {AuthorId}.", editorProfile.Id, comment.Id, comment.AuthorId);
                throw new UnauthorizedException(
                    $"User {editorProfile.Id} is not authorized to edit comment {commentId}.");
            }

            if (!string.IsNullOrWhiteSpace(request.Content) && comment.Content != request.Content.Trim())
            {
                comment.Content = request.Content.Trim();

                await _dbContext.SaveChangesAsync(); // Save only if actual changes were made

                // Offload to inappropriate content detection
                await _inappropriateCommentChannel.WriteAsync(new DetectInappropriateCommentMessage
                {
                    CommentId = comment.Id,
                    Content = comment.Content,
                    AuthorId = comment.AuthorId,
                });
            }

            // Return the potentially updated comment, mapped to response
            // Re-fetch with full population to ensure response is complete.
            var updatedCommentEntity = await _dbContext.Comments
                .FullyPopulatedCommentQuery(c => c.Id == commentId)
                .SelectCommentResponseFromFullComment(editorProfile.Id)
                .FirstOrDefaultAsync();

            return updatedCommentEntity;
        }

        public async Task<CommentResponse> VoteCommentAsync(string commentId, Profile voterProfile, bool isUpvoteIntent)
        {
            ArgumentException.ThrowIfNullOrEmpty(commentId);
            ArgumentNullException.ThrowIfNull(voterProfile);

            // Fetch comment with necessary includes for vote processing and notification
            var comment = await _dbContext.Comments
                .Include(c => c.Author) // Needed for notification recipient
                                        // .Include(c => c.Votes) // Not strictly needed here if we query CommentVotes separately
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null)
            {
                throw new CommentNotFoundException($"Comment with id={commentId} was not found for voting.");
            }

            var vote = await _dbContext.CommentVotes
                // .Include(v => v.Comment) // Not needed if comment is already fetched
                // .ThenInclude(c => c.Author)
                .FirstOrDefaultAsync(v => v.CommentId == commentId && v.ProfileId == voterProfile.Id);

            if (vote == null)
            {
                vote = new CommentVote
                {
                    ProfileId = voterProfile.Id,
                    Profile = voterProfile, 
                    CommentId = comment.Id,
                    Comment = comment,   
                    IsUpvote = isUpvoteIntent,
                };

                await _dbContext.CommentVotes.AddAsync(vote);
            }
            else
            {
                if (vote.IsUpvote == isUpvoteIntent) // upvoting an already upvoted comment
                {
                    _dbContext.CommentVotes.Remove(vote);
                    vote = null;
                }
                else // upvoting a downvoted comment, or downvoting an upvoted one
                {
                    vote.IsUpvote = isUpvoteIntent;
                }
            }

            await _dbContext.SaveChangesAsync();

            if (vote != null) 
            {
                // Ensure voteToNotify.Comment and voteToNotify.Profile (voterProfile) are loaded for the facade
                // voteToNotify.Comment is 'comment' which has Author loaded.
                // voteToNotify.Profile will be voterProfile.

                vote.Comment = comment;
                await _notificationFacade.NotifyCommentVotedAsync(vote);
            }

            // Re-fetch the comment with all details for the response
            var updatedComment = await _dbContext.Comments
                .FullyPopulatedCommentQuery(c => c.Id == commentId)
                .SelectCommentResponseFromFullComment(voterProfile.Id)
                .FirstAsync(); 

            return updatedComment;
        }


        // TODO: CONSIDER!
        // Do we need to delete the replies on this comment? 
        public async Task<bool> DeleteCommentAsync(string commentId, Profile deleterProfile)
        {
            ArgumentException.ThrowIfNullOrEmpty(commentId, nameof(commentId));
            ArgumentNullException.ThrowIfNull(deleterProfile, nameof(deleterProfile));

            var comment = await _dbContext.Comments
                .Include(c => c.Replies)
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null)
            {
                // Don't throw if not found for DELETE, just return false (idempotency)
                // Or, if you prefer to throw, the controller should catch and return NoContent still.
                // For strict idempotency, "resource not found" is a success state for DELETE.
                return true; // Or false if you want to signal "wasn't there to delete"
            }

            if (comment.AuthorId != deleterProfile.Id)
            {
                //Optionally, check for admin / moderator roles if they can delete others' comments
                //if (!await _userService.IsAdminOrModeratorAsync(deleterProfile))
                //{
                //    _logger.LogWarning("User {DeleterProfileId} attempted to delete comment {CommentId} owned by {AuthorId}.", deleterProfile.Id, comment.Id, comment.AuthorId);
                //    throw new ForbiddenAccessException($"User {deleterProfile.Id} is not authorized to delete comment {commentId}.");
                //}

                throw new ForbiddenAccessException($"User {deleterProfile.Id} is not authorized to delete comment {commentId}.");
            }

            _dbContext.Comments.Remove(comment);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        private string SanitizeMediaName(string contentHint, int maxLength = 50)
        {
            if (string.IsNullOrWhiteSpace(contentHint)) return "Untitled";
            var name = contentHint.Trim();
            if (name.Length > maxLength)
            {
                name = name.Substring(0, maxLength);
            }
            // Further sanitization (e.g., remove invalid characters for filenames) if needed
            return name;
        }
    }
}
