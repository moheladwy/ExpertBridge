// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Core.Entities.Comments;
using ExpertBridge.Api.Responses;

namespace ExpertBridge.Api.Queries
{
    public static class CommentQueries
    {
        public static IEnumerable<CommentResponse> SelectCommentResponseFromFullComment(
            this IEnumerable<Comment> query,
            string? userProfileId)
        {
            bool hasReplies = query.Any(c => c.Replies.Count > 0);

            return query
                .Select(c => new CommentResponse
                {
                    Id = c.Id,
                    Content = c.Content,
                    Author = c.Author.SelectAuthorResponseFromProfile(),
                    Upvotes = c.Votes.Count(v => v.IsUpvote),
                    Downvotes = c.Votes.Count(v => !v.IsUpvote),
                    IsUpvoted = c.Votes.Any(v => v.IsUpvote && v.ProfileId == userProfileId),
                    IsDownvoted = c.Votes.Any(v => !v.IsUpvote && v.ProfileId == userProfileId),
                    CreatedAt = c.CreatedAt,
                    Replies = hasReplies
                        ? c.Replies.SelectCommentResponseFromFullComment(userProfileId).ToList()
                        : []
                });
        }
    }
}
