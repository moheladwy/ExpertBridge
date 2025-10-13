// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Requests;

public class EditJobPostingRequest
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public decimal? Budget { get; set; }
    public string? Area { get; set; }
}
