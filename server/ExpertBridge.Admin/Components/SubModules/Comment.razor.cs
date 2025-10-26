// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Responses;
using Microsoft.AspNetCore.Components;

namespace ExpertBridge.Admin.Components.SubModules;

public partial class Comment : ComponentBase
{
    private bool _showReplies = true;

    [Parameter] public CommentResponse CommentResponse { get; set; }

    private void ToggleReplies()
    {
        _showReplies = !_showReplies;
    }
}
