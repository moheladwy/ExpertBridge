// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq.Expressions;
using ExpertBridge.Contract.Responses;
using ExpertBridge.Core.Entities.Comments;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Contract.Queries;

/// <summary>
///     Provides extension methods for querying and projecting Comment entities.
/// </summary>
/// <remarks>
///     These query extensions enable reusable patterns for loading related data including nested replies
///     and projecting to response DTOs with user-specific vote states.
/// </remarks>
public static class CommentQueries
{
    /// <summary>
    ///     Eagerly loads all related data for top-level comments including author, votes, media, and nested replies.
    /// </summary>
    /// <param name="query">The source queryable of comments.</param>
    /// <returns>A queryable of top-level comments with all navigation properties included.</returns>
    /// <remarks>
    ///     Filters to only top-level comments (ParentCommentId == null).
    ///     Uses AsNoTracking for read-only queries. Includes: Votes, Author, Medias, Replies with Authors.
    /// </remarks>
    public static IQueryable<Comment> FullyPopulatedCommentQuery(this IQueryable<Comment> query)
    {
        return query
            .AsNoTracking()
            .AsSplitQuery()
            .Where(c => c.ParentCommentId == null)
            .Include(c => c.Votes)
            .Include(c => c.Author)
            .Include(c => c.Medias)
            .Include(c => c.Replies)
            .ThenInclude(r => r.Author);
    }

    /// <summary>
    ///     Eagerly loads all related data for comments and filters by the specified predicate.
    /// </summary>
    /// <param name="query">The source queryable of comments.</param>
    /// <param name="predicate">The filter expression to apply.</param>
    /// <returns>A filtered queryable with all navigation properties included.</returns>
    public static IQueryable<Comment> FullyPopulatedCommentQuery(
        this IQueryable<Comment> query,
        Expression<Func<Comment, bool>> predicate)
    {
        return query
            .FullyPopulatedCommentQuery()
            .Where(predicate);
    }

    /// <summary>
    ///     Projects a queryable of comments to CommentResponse DTOs with user-specific vote information.
    /// </summary>
    /// <param name="query">The source queryable of comments.</param>
    /// <param name="userProfileId">The ID of the current user for determining vote states.</param>
    /// <returns>A queryable of CommentResponse objects with nested replies.</returns>
    public static IQueryable<CommentResponse> SelectCommentResponseFromFullComment(
        this IQueryable<Comment> query,
        string? userProfileId)
    {
        return query
            .Select(c => SelectCommentResponseFromFullComment(c, userProfileId));
    }

    /// <summary>
    ///     Projects a single comment entity to a CommentResponse DTO with user-specific vote information and nested replies.
    /// </summary>
    /// <param name="c">The comment entity to project.</param>
    /// <param name="userProfileId">The ID of the current user for determining vote states.</param>
    /// <returns>
    /// A CommentResponse object with calculated vote counts, user vote states, and recursively projected replies.
    /// </returns>
    /// <remarks>
    ///     Only the replies already loaded onto <see cref="Comment.Replies" /> (typically a single level via
    ///     <see cref="FullyPopulatedCommentQuery(IQueryable{Comment})" />) are projected. To assemble a fully-nested
    ///     tree from a flat list of comments at every depth, use <see cref="BuildCommentTree" /> instead.
    /// </remarks>
    public static CommentResponse SelectCommentResponseFromFullComment(
        this Comment c,
        string? userProfileId)
    {
        var replies = c.Replies
            .OrderBy(r => r.CreatedAt)
            .Select(r => r.SelectCommentResponseFromFullComment(userProfileId))
            .ToList();

        return MapScalar(c, userProfileId, replies);
    }

    /// <summary>
    ///     Builds fully-nested <see cref="CommentResponse" /> trees from a flat collection of comments
    ///     (top-level comments and replies at every depth) using an in-memory parent lookup.
    /// </summary>
    /// <param name="comments">The flat collection of comments to assemble, e.g. all comments for a single post.</param>
    /// <param name="userProfileId">The ID of the current user for determining vote states.</param>
    /// <returns>The top-level comments (ParentCommentId == null), each with its replies nested recursively.</returns>
    /// <remarks>
    ///     Avoids the cartesian explosion of eagerly including <see cref="Comment.Replies" /> and nests replies
    ///     correctly at all depths. Each level is ordered by CreatedAt ascending. A visited set guards against
    ///     accidental cycles. Replies whose parent is absent from <paramref name="comments" /> (e.g. a soft-deleted
    ///     parent) are unreachable from a root and are therefore omitted.
    /// </remarks>
    public static List<CommentResponse> BuildCommentTree(
        this IReadOnlyCollection<Comment> comments,
        string? userProfileId)
    {
        var byParent = comments.ToLookup(c => c.ParentCommentId);

        List<CommentResponse> Build(string? parentId, HashSet<string> visited)
        {
            return byParent[parentId]
                .OrderBy(c => c.CreatedAt)
                .Where(c => visited.Add(c.Id)) // cycle guard
                .Select(c => MapScalar(c, userProfileId, Build(c.Id, visited)))
                .ToList();
        }

        return Build(null, []);
    }

    /// <summary>
    ///     Maps the scalar fields of a comment, together with its already-built replies, to a <see cref="CommentResponse" />.
    /// </summary>
    private static CommentResponse MapScalar(Comment c, string? userProfileId, List<CommentResponse> replies)
    {
        return new CommentResponse
        {
            Id = c.Id,
            Content = c.Content,
            Author = c.Author.SelectAuthorResponseFromProfile(),
            AuthorId = c.AuthorId,
            PostId = c.PostId,
            JobPostingId = c.JobPostingId,
            ParentCommentId = c.ParentCommentId,
            LastModified = c.UpdatedAt,
            Upvotes = c.Votes.Count(v => v.IsUpvote),
            Downvotes = c.Votes.Count(v => !v.IsUpvote),
            IsUpvoted = c.Votes.Any(v => v.IsUpvote && v.ProfileId == userProfileId),
            IsDownvoted = c.Votes.Any(v => !v.IsUpvote && v.ProfileId == userProfileId),
            CreatedAt = c.CreatedAt.Value,
            Medias = c.Medias.AsQueryable().SelectMediaObjectResponse().ToList(),
            Replies = replies
        };
    }
}
