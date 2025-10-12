using System.Text;
using System.Threading.Channels;
using ExpertBridge.Application.EmbeddingService;
using ExpertBridge.Contract.Messages;
using ExpertBridge.Core.Exceptions;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ExpertBridge.Api.BackgroundServices.Handlers
{
    public class UserInterestsUpdatedHandlerWorker
        : HandlerWorker<UserInterestsUpdatedHandlerWorker, UserInterestsUpdatedMessage>
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<UserInterestsUpdatedHandlerWorker> _logger;

        public UserInterestsUpdatedHandlerWorker(
            IServiceProvider services,
            ILogger<UserInterestsUpdatedHandlerWorker> logger,
            Channel<UserInterestsUpdatedMessage> channel)
            : base(nameof(UserInterestsUpdatedHandlerWorker), channel.Reader, logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteInternalAsync(
            UserInterestsUpdatedMessage message,
            CancellationToken stoppingToken)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(message, nameof(message));
                using var scope = _services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ExpertBridgeDbContext>();
                var embeddingService = scope.ServiceProvider.GetRequiredService<IEmbeddingService>();

                var userInterests = dbContext.UserInterests
                    .AsNoTracking()
                    .Include(ui => ui.Tag)
                    .Where(ui => ui.ProfileId == message.UserProfileId)
                    .Select(ui => $"{ui.Tag.EnglishName} {ui.Tag.ArabicName} {ui.Tag.Description} ");

                var text = new StringBuilder();
                foreach (var ui in userInterests)
                {
                    text.Append(ui);
                }

                var embedding = await embeddingService.GenerateEmbedding(text.ToString());

                if (embedding is null)
                {
                    throw new RemoteServiceCallFailedException(
                        $"Error: Embedding service returned null embedding for user=${message.UserProfileId}.");
                }

                var user = await dbContext.Profiles
                    .FirstOrDefaultAsync(p => p.Id == message.UserProfileId, stoppingToken);

                if (user is not null)
                {
                    user.UserInterestEmbedding = embedding;
                    await dbContext.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, $"An error occurred while processing " +
                //     $"message with user profile id={message.UserProfileId}.");
                Log.Error(ex,
                    "An error occurred while processing message with user profile id={userProfileId}.",
                    message.UserProfileId);
            }

        }
    }
}
