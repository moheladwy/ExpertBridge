// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Contract.Queries;
using ExpertBridge.Contract.Responses;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Admin.Components.SubModules;

/// <summary>
///     This is the code-behind file for the JobPost component.
/// </summary>
public partial class JobPost : ComponentBase
{
    private readonly ExpertBridgeDbContext _dbContext;
    private List<CommentResponse>? _comments;
    private bool _loadingComments;
    private bool _showComments;

    public JobPost(ExpertBridgeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    ///     The job posting data to be displayed in the component.
    /// </summary>
    [Parameter]
    public JobPostingResponse? JobPostingResponse { get; set; }

    private async Task ToggleCommentsAsync()
    {
        _showComments = !_showComments;

        // Load comments if showing and not already loaded
        if (_showComments && _comments == null && JobPostingResponse != null)
        {
            await LoadCommentsAsync();
        }
    }

    private async Task LoadCommentsAsync()
    {
        if (JobPostingResponse == null)
        {
            return;
        }

        _loadingComments = true;
        StateHasChanged();

        try
        {
            _comments = await _dbContext.Comments
                .IgnoreQueryFilters()
                .FullyPopulatedCommentQuery(c => c.JobPostingId == JobPostingResponse.Id)
                .SelectCommentResponseFromFullComment(null)
                .ToListAsync();
        }
#pragma warning disable CA1031
        catch
#pragma warning restore CA1031
        {
            _comments = [];
        }
        finally
        {
            _loadingComments = false;
            StateHasChanged();
        }
    }
}
