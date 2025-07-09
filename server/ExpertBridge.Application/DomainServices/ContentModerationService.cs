using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;

namespace ExpertBridge.Application.DomainServices
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
            InappropriateLanguageDetectionResponse results,
            bool isNegative)
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
