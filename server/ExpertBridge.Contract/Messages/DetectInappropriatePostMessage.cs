// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Contract.Messages
{
    public class DetectInappropriatePostMessage
    {
        public string PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string AuthorId { get; set; }
        public required bool IsJobPosting { get; set; }
    }
}
