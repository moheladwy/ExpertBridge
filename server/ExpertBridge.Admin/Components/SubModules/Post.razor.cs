// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Queries;
using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Admin.Components.SubModules;

/// <summary>
///     This is the code-behind file for the Post component.
/// </summary>
public partial class Post : ComponentBase
{
    private readonly ExpertBridgeDbContext _dbContext;
    private List<CommentResponse>? _comments;
    private bool _loadingComments;
    private bool _showComments;

    public Post(ExpertBridgeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    ///     The post-data to be displayed in the component.
    /// </summary>
    [Parameter]
    public PostResponse? PostResponse { get; set; }

    private async Task ToggleCommentsAsync()
    {
        _showComments = !_showComments;

        // Load comments if showing and not already loaded
        if (_showComments && _comments == null && PostResponse != null)
        {
            await LoadCommentsAsync();
        }
    }

    private async Task LoadCommentsAsync()
    {
        if (PostResponse == null)
        {
            return;
        }

        _loadingComments = true;
        StateHasChanged();

        try
        {
            _comments = await _dbContext.Comments
                .IgnoreQueryFilters()
                .FullyPopulatedCommentQuery(c => c.PostId == PostResponse.Id)
                .SelectCommentResponseFromFullComment(null)
                .ToListAsync();
        }
        catch
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
