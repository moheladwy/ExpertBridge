// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Responses
{
    public class PostsCursorPaginatedResponse
    {
        public List<PostResponse> Posts { get; set; }
        public PageInfoResponse PageInfo { get; set; }
    }
}
