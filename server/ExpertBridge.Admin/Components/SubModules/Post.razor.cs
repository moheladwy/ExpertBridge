// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Responses;
using Microsoft.AspNetCore.Components;

namespace ExpertBridge.Admin.Components.SubModules
{
    /// <summary>
    /// This is the code-behind file for the Post component.
    /// </summary>
    public partial class Post : ComponentBase
    {
        /// <summary>
        ///   The post-data to be displayed in the component.
        /// </summary>
        [Parameter]
        public PostResponse? PostResponse { get; set; }
    }
}
