// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq.Expressions;
using ExpertBridge.Contract.Responses;
using ExpertBridge.Core.Entities.Posts;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Contract.Queries;

/// <summary>
///     Provides extension methods for querying and projecting Post entities.
/// </summary>
/// <remarks>
///     These query extensions enable reusable patterns for loading related data
///     and projecting to response DTOs with user-specific vote states.
/// </remarks>
public static class PostQueries
{
    /// <summary>
    ///     Eagerly loads all related data for posts including author, votes, media, comments, and tags.
    /// </summary>
    /// <param name="query">The source queryable of posts.</param>
    /// <returns>A queryable of posts with all navigation properties included.</returns>
    /// <remarks>
    ///     Uses AsNoTracking for read-only queries. Includes: Author, Votes, Medias, Comments, PostTags with Tags.
    /// </remarks>
    public static IQueryable<Post> FullyPopulatedPostQuery(this IQueryable<Post> query)
    {
        return query
            .AsNoTracking()
            .Include(p => p.Author)
            .Include(p => p.Votes)
            .Include(p => p.Medias)
            .Include(p => p.Comments)
            .Include(p => p.PostTags)
            .ThenInclude(pt => pt.Tag);
    }

    //.ThenInclude(c => c.Author)
    //.Include(p => p.Comments)
    //.ThenInclude(c => c.Replies)
    //.ThenInclude(c => c.Author)
    /// <summary>
    ///     Eagerly loads all related data for posts and filters by the specified predicate.
    /// </summary>
    /// <param name="query">The source queryable of posts.</param>
    /// <param name="predicate">The filter expression to apply.</param>
    /// <returns>A filtered queryable with all navigation properties included.</returns>
    public static IQueryable<Post> FullyPopulatedPostQuery(this IQueryable<Post> query,
        Expression<Func<Post, bool>> predicate)
    {
        return query
            .FullyPopulatedPostQuery()
            .Where(predicate);
    }

    /// <summary>
    ///     Projects a queryable of Post entities to PostResponse DTOs with user-specific vote information.
    /// </summary>
    /// <param name="query">The source queryable of posts.</param>
    /// <param name="userProfileId">The ID of the current user for determining vote states, or null for anonymous users.</param>
    /// <returns>A queryable of PostResponse objects with vote counts, tags, and media.</returns>
    public static IQueryable<PostResponse> SelectPostResponseFromFullPost(
        this IQueryable<Post> query,
        string? userProfileId)
    {
        return query
            .Select(p => SelectPostResponseFromFullPost(p, userProfileId));
    }

    /// <summary>
    ///     Projects a single Post entity to a PostResponse DTO with user-specific vote information.
    /// </summary>
    /// <param name="p">The post entity to project.</param>
    /// <param name="userProfileId">The ID of the current user for determining vote states, or null for anonymous users.</param>
    /// <returns>A PostResponse object with vote counts, tags, media, and whether the user has voted.</returns>
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
}
