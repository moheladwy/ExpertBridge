using System.Linq.Expressions;
using ExpertBridge.Core.Entities.Posts;
using ExpertBridge.Core.Responses;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Core.Queries
{
    public static class PostQueries
    {
        public static IQueryable<Post> FullyPopulatedPostQuery(this IQueryable<Post> query)
        {
            return query
                .AsNoTracking()
                .Include(p => p.Author)
                .Include(p => p.Votes)
                .Include(p => p.Medias)
                .Include(p => p.Comments)
                //.ThenInclude(c => c.Author)
                //.Include(p => p.Comments)
                //.ThenInclude(c => c.Replies)
                //.ThenInclude(c => c.Author)
                ;
        }

        public static IQueryable<Post> FullyPopulatedPostQuery(this IQueryable<Post> query,
            Expression<Func<Post, bool>> predicate)
        {
            return query
                .FullyPopulatedPostQuery()
                .Where(predicate);
        }

        public static IQueryable<PostResponse> SelectPostResponseFromFullPost(
            this IQueryable<Post> query,
            string? userProfileId)
        {
            return query
                .Select(p => SelectPostResponseFromFullPost(p, userProfileId));
        }

        public static PostResponse SelectPostResponseFromFullPost(
            this Post p,
            string? userProfileId)
        {
            return new PostResponse
            {
                IsUpvoted = p.Votes.Any(v => v.IsUpvote && v.ProfileId == userProfileId),
                IsDownvoted = p.Votes.Any(v => !v.IsUpvote && v.ProfileId == userProfileId),

                Title = p.Title,
                Content = p.Content,
                Author = p.Author.SelectAuthorResponseFromProfile(),
                CreatedAt = p.CreatedAt.Value,
                Id = p.Id,
                Upvotes = p.Votes.Count(v => v.IsUpvote),
                Downvotes = p.Votes.Count(v => !v.IsUpvote),
                Comments = p.Comments.Count,
                Medias = p.Medias.AsQueryable().SelectMediaObjectResponse().ToList(),
            };
        }
    }
}
