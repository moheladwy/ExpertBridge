// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Linq.Expressions;
using ExpertBridge.Core.Entities.Posts;
using ExpertBridge.Core.Responses;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Core.Queries;

public static class PostQueries
{
    public static IQueryable<Post> FullyPopulatedPostQuery(this IQueryable<Post> query) =>
        query
            .AsNoTracking()
            .Include(p => p.Author)
            .Include(p => p.Votes)
            .Include(p => p.Medias)
            .Include(p => p.Comments)
            .Include(p => p.PostTags)
            .ThenInclude(pt => pt.Tag);

    //.ThenInclude(c => c.Author)
    //.Include(p => p.Comments)
    //.ThenInclude(c => c.Replies)
    //.ThenInclude(c => c.Author)
    public static IQueryable<Post> FullyPopulatedPostQuery(this IQueryable<Post> query,
        Expression<Func<Post, bool>> predicate) =>
        query
            .FullyPopulatedPostQuery()
            .Where(predicate);

    public static IQueryable<PostResponse> SelectPostResponseFromFullPost(
        this IQueryable<Post> query,
        string? userProfileId) =>
        query
            .Select(p => SelectPostResponseFromFullPost(p, userProfileId));

    public static PostResponse SelectPostResponseFromFullPost(
        this Post p,
        string? userProfileId) =>
        new()
        {
            IsUpvoted = p.Votes.Any(v => v.IsUpvote && v.ProfileId == userProfileId),
            IsDownvoted = p.Votes.Any(v => !v.IsUpvote && v.ProfileId == userProfileId),
            Title = p.Title,
            Content = p.Content,
            Language = p.Language,
            Author = p.Author.SelectAuthorResponseFromProfile(),
            CreatedAt = p.CreatedAt.Value,
            LastModified = p.UpdatedAt,
            Id = p.Id,
            Upvotes = p.Votes.Count(v => v.IsUpvote),
            Downvotes = p.Votes.Count(v => !v.IsUpvote),
            Comments = p.Comments.Count,
            Tags = p.PostTags.Select(pt => pt.Tag.SelectTagResponseFromTag()).ToList(),
            Medias = p.Medias.AsQueryable().SelectMediaObjectResponse().ToList()
        };
}
