// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Core.Entities.Comments;
using ExpertBridge.Api.Core.Entities.Posts;
using ExpertBridge.Api.Responses;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Api.Queries
{
    public static class CommentQueries
    {
        public static IQueryable<Comment> FullyPopulatedCommentQuery(this IQueryable<Comment> query)
        {
            return query
                .Include(c => c.Author)
                .Include(c => c.Replies)
                .ThenInclude(r => r.Author)
                ;
        }

        public static IQueryable<CommentResponse> SelectCommentResponseFromFullComment(
            this IQueryable<Comment> query,
            string? userProfileId)
        {
            bool hasReplies = query.Any(c => c.Replies.Count > 0);

            return query
                .Select(c => SelectCommentResponseFromFullComment(c, userProfileId));
        }

        public static CommentResponse SelectCommentResponseFromFullComment(
            this Comment c,
            string? userProfileId)
        {
            return new CommentResponse
            {
                Id = c.Id,
                Content = c.Content,
                Author = c.Author.SelectAuthorResponseFromProfile(),
                AuthorId = c.AuthorId,
                PostId = c.PostId,
                ParentCommentId = c.ParentCommentId,
                Upvotes = c.Votes.Count(v => v.IsUpvote),
                Downvotes = c.Votes.Count(v => !v.IsUpvote),
                IsUpvoted = c.Votes.Any(v => v.IsUpvote && v.ProfileId == userProfileId),
                IsDownvoted = c.Votes.Any(v => !v.IsUpvote && v.ProfileId == userProfileId),
                CreatedAt = c.CreatedAt,
                Replies = c.Replies
                            .AsQueryable()
                            .SelectCommentResponseFromFullComment(userProfileId)
                            .ToList()
            };
        }
    }
}
