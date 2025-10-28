// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Contract.Queries;
using ExpertBridge.Contract.Responses;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Radzen;

namespace ExpertBridge.Admin.Components.Pages.MainContent;

public partial class Comments : ComponentBase
{
    private readonly int pageSize = 4;

    private List<CommentResponse> comments = [];
    private int count;
    private List<CommentResponse>? filteredComments;
    private int filteredCount;
    private List<CommentResponse> pagedComments = [];

    // Search properties
    private string searchText = string.Empty;

    [Inject] private ExpertBridgeDbContext DbContext { get; set; } = default!;

    [Inject] private HybridCache Cache { get; set; } = default!;

    private int displayedCount
    {
        get { return string.IsNullOrWhiteSpace(searchText) ? count : filteredCount; }
    }

    private string pagingSummaryFormat
    {
        get
        {
            return string.IsNullOrWhiteSpace(searchText)
                ? "Displaying page {0} of {1} <b>(total {2} comments)</b>"
                : "Displaying page {0} of {1} <b>({2} comments found, {3} total)</b>";
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadCommentsAsync();
    }

    private async Task LoadCommentsAsync()
    {
        comments = await Cache.GetOrCreateAsync(
            "admin:comments:all",
            async cancel => await DbContext.Comments
                .Where(c => !c.IsDeleted)
                .FullyPopulatedCommentQuery()
                .OrderByDescending(c => c.CreatedAt)
                .SelectCommentResponseFromFullComment(null)
                .ToListAsync(cancel),
            cancellationToken: default
        );

        count = comments.Count;
        UpdatePaged(0, pageSize);
    }

    private void UpdatePaged(int skip, int take)
    {
        var sourceList = string.IsNullOrWhiteSpace(searchText) ? comments : filteredComments ?? comments;
        pagedComments = sourceList
            .Skip(skip)
            .Take(take)
            .ToList();
    }

    private void PageChanged(PagerEventArgs args)
    {
        UpdatePaged(args.Skip, args.Top);
    }

    private List<CommentResponse> GetFilteredComments()
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return comments;
        }

        return comments.Where(c => CommentMatchesSearch(c, searchText)).ToList();
    }

    private static bool CommentMatchesSearch(CommentResponse comment, string searchTerm)
    {
        // Check if this comment matches
        if (CommentFieldsMatch(comment, searchTerm))
        {
            return true;
        }

        // Recursively check replies
        if (comment.Replies != null && comment.Replies.Any(reply => CommentMatchesSearch(reply, searchTerm)))
        {
            return true;
        }

        return false;
    }

    private static bool CommentFieldsMatch(CommentResponse comment, string searchTerm)
    {
        return (comment.Id?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
               (comment.Content?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
               (comment.Author?.Username?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
               (comment.Author?.FirstName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
               (comment.Author?.LastName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
               (comment.PostId?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
               (comment.JobPostingId?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false);
    }

    private void OnSearchChanged(string value)
    {
        searchText = value;
        filteredComments = GetFilteredComments();
        filteredCount = filteredComments.Count;
        UpdatePaged(0, pageSize);
    }

    private void ClearSearch()
    {
        searchText = string.Empty;
        filteredComments = null;
        filteredCount = 0;
        UpdatePaged(0, pageSize);
    }
}
