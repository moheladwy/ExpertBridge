// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Requests
{
    public class PostsCursorRequest
    {
        public int PageSize { get; set; } = 10;
        public double? After { get; set; }
        public string? LastIdCursor { get; set; }
    }
}
