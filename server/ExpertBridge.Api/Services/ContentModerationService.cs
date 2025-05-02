// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;

namespace ExpertBridge.Api.Services
{
    public class ContentModerationService
    {
        private readonly ExpertBridgeDbContext _dbContext;

        public ContentModerationService(ExpertBridgeDbContext dbContext)
        {
            // Get a channel to an email sending service
            // to delegate email sending to it.
            _dbContext = dbContext;
        }

        public async Task ReportPostAsync(
            string postId,
            InappropriateLanguageDetectionResponse results)
        {

        }

        public async Task ReportPostAsync(string postId, string reason)
        {

        }

        public async Task ReportCommentAsync(
            string commentId,
            InappropriateLanguageDetectionResponse results)
        {

        }

        public async Task ReportCommentAsync(string commentId, string reason)
        {

        }
    }
}
