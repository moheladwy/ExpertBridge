using System.Linq.Expressions;
using ExpertBridge.Core.Entities.Comments;
using ExpertBridge.Core.Responses;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Data.Queries
{
    public static class CommentQueries
    {
        public static IQueryable<Comment> FullyPopulatedCommentQuery(this IQueryable<Comment> query)
        {
            return query
                .AsNoTracking()
                .Where(c => c.ParentCommentId == null)
                .Include(c => c.Votes)
                .Include(c => c.Author)
                .Include(c => c.Medias)
                .Include(c => c.Replies)
                .ThenInclude(r => r.Author)
                ;
        }

        public static IQueryable<Comment> FullyPopulatedCommentQuery(
            this IQueryable<Comment> query,
            Expression<Func<Comment, bool>> predicate)
        {
            return query
                .FullyPopulatedCommentQuery()
                .Where(predicate)
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
                CreatedAt = c.CreatedAt.Value,
                Medias = c.Medias.AsQueryable().SelectMediaObjectResponse().ToList(),
                Replies = c.Replies
                            .AsQueryable()
                            .OrderBy(c => c.CreatedAt)
                            .SelectCommentResponseFromFullComment(userProfileId)
                            .ToList()
            };
        }
    }
}
