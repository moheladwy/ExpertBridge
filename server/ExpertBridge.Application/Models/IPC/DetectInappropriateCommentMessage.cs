// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Application.Models.IPC
{
    public class DetectInappropriateCommentMessage
    {
        public string CommentId { get; set; }
        public string Content { get; set; }
        public string AuthorId { get; set; }
    }
}
