// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Responses;

public class PageInfoResponse
{
    public string? NextIdCursor { get; set; }
    public double? EndCursor { get; set; }
    public bool HasNextPage { get; set; }
    public string? Embedding { get; set; }
}
