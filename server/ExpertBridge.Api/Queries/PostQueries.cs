// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq;
using ExpertBridge.Api.Core.Entities.Posts;
using ExpertBridge.Api.Responses;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Api.Queries
{
    public static class PostQueries
    {
        public static IQueryable<Post> FullyPopulatedPostQuery(this IQueryable<Post> query)
        {
            return query
                .Where(p => !p.IsDeleted)
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

        public static IQueryable<PostResponse> SelectPostResponseFromFullPost(
            this IQueryable<Post> query,
            string? userProfileId)
        {
            return query
                .Select(p => new PostResponse
                {
                    IsUpvoted = p.Votes.Any(v => v.IsUpvote && v.ProfileId == userProfileId),
                    IsDownvoted = p.Votes.Any(v => !v.IsUpvote && v.ProfileId == userProfileId),

                    Title = p.Title,
                    Content = p.Content,
                    Author = p.Author.SelectAuthorResponseFromProfile(),
                    CreatedAt = p.CreatedAt,
                    Id = p.Id,
                    Upvotes = p.Votes.Count(v => v.IsUpvote),
                    Downvotes = p.Votes.Count(v => !v.IsUpvote),
                    Comments = p.Comments.Count
                    //Comments = p.Comments.SelectSelect(c => new CommentResponse
                    //{
                    //    Id = c.Id,
                    //    Content = c.Content,
                    //    Author = new AuthorResponse
                    //    {
                    //        Id = c.AuthorId,
                    //        JobTitle = c.Author.JobTitle,
                    //        ProfilePictureUrl = c.Author.ProfilePictureUrl,
                    //        UserId = c.Author.UserId
                    //    },
                    //    CreatedAt = c.CreatedAt,
                    //    Replies = c.Replies.Select(cc => new CommentResponse
                    //    {
                    //        Id = cc.Id,
                    //        Content = cc.Content,
                    //        Author = new AuthorResponse
                    //        {
                    //            Id = cc.AuthorId,
                    //            JobTitle = cc.Author.JobTitle,
                    //            ProfilePictureUrl = cc.Author.ProfilePictureUrl,
                    //            UserId = cc.Author.UserId
                    //        },
                    //        CreatedAt = c.CreatedAt,
                    //        Replies = null
                    //    }).ToList()
                    //}).ToList()
                });
        }
    }
}
