// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Queries;
using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Radzen;

namespace ExpertBridge.Admin.Components.Pages.DeletedContent;

public partial class DeletedComments : ComponentBase
{
    private readonly HybridCache _cache;
    private readonly ExpertBridgeDbContext _dbContext;
    private readonly int pageSize = 4;
    private List<CommentResponse>? filteredComments;
    private int filteredCount;
    private bool isLoading = true;
    private List<CommentResponse>? pagedComments;

    // Search properties
    private string searchText = string.Empty;

    public DeletedComments(ExpertBridgeDbContext dbContext, HybridCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
        Comments = [];
        pagedComments = [];
    }

    public List<CommentResponse> Comments { get; set; }
    private int displayedCommentCount
    {
        get { return string.IsNullOrWhiteSpace(searchText) ? Comments.Count : filteredCount; }
    }

    private string pagingSummaryFormat
    {
        get
        {
            return
                $"Displaying page {{0}} of {{1}} (total {{2}} {(string.IsNullOrWhiteSpace(searchText) ? "deleted comments" : "results")})";
        }
    }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            isLoading = true;
            Comments = await GetDeletedComments();
            UpdatePagedComments(0, pageSize);
        }
        finally
        {
            isLoading = false;
        }

        await base.OnInitializedAsync();
    }

    private void OnPageChanged(PagerEventArgs args)
    {
        UpdatePagedComments(args.Skip, args.Top);
    }

    private List<CommentResponse> GetFilteredComments()
    {
        if (Comments == null || string.IsNullOrWhiteSpace(searchText))
        {
            return Comments ?? [];
        }

        return Comments.Where(c => CommentMatchesSearch(c, searchText)).ToList();
    }

    private static bool CommentMatchesSearch(CommentResponse comment, string search)
    {
        // Check if the comment itself matches
        if (CommentFieldsMatch(comment, search))
        {
            return true;
        }

        // Recursively check if any nested reply matches
        if (comment.Replies != null && comment.Replies.Count > 0)
        {
            return comment.Replies.Any(reply => CommentMatchesSearch(reply, search));
        }

        return false;
    }

    private static bool CommentFieldsMatch(CommentResponse comment, string search)
    {
        return (comment.Id?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
               (comment.Content?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
               (comment.Author?.Username?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
               (comment.Author?.FirstName?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
               (comment.Author?.LastName?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
               (comment.PostId?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
               (comment.JobPostingId?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false);
    }

    private void OnSearchChanged(string value)
    {
        searchText = value;
        filteredComments = GetFilteredComments();
        filteredCount = filteredComments.Count;
        UpdatePagedComments(0, pageSize); // Reset to first page when search changes
    }

    private void ClearSearch()
    {
        searchText = string.Empty;
        OnSearchChanged(string.Empty);
    }

    private void UpdatePagedComments(int skip, int take)
    {
        var source = string.IsNullOrWhiteSpace(searchText) ? Comments : filteredComments;
        if (source != null)
        {
            pagedComments = source.Skip(skip).Take(take).ToList();
        }
    }

    private async Task<List<CommentResponse>> GetDeletedComments()
    {
        const string cacheKey = "deleted-comments";
        var cachedComments = await _cache.GetOrCreateAsync<List<CommentResponse>>(cacheKey, async cancellationToken =>
        {
            var comments = await _dbContext.Comments
                .IgnoreQueryFilters()
                .FullyPopulatedCommentQuery()
                .SelectCommentResponseFromFullComment(null)
                .ToListAsync(cancellationToken);
            return comments;
        });
        return cachedComments;
    }
}
