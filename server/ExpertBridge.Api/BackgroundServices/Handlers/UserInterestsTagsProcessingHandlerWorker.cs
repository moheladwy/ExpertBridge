using System.Threading.Channels;
using ExpertBridge.Api.Models.IPC;
using ExpertBridge.Api.Services;
using ExpertBridge.Core.Entities.ManyToManyRelationships.UserInterests;
using ExpertBridge.Core.Entities.Tags;
using ExpertBridge.Core.Exceptions;
using ExpertBridge.Data.DatabaseContexts;
using Serilog;

namespace ExpertBridge.Api.BackgroundServices.Handlers;

public sealed class UserInterestsTagsProcessingHandlerWorker
    : HandlerWorker<UserInterestsTagsProcessingHandlerWorker, UserInterestsProsessingMessage>
{
    private readonly IServiceProvider _services;
    private readonly ILogger<UserInterestsTagsProcessingHandlerWorker> _logger;

    public UserInterestsTagsProcessingHandlerWorker(
        IServiceProvider services,
        ChannelReader<UserInterestsProsessingMessage> channel,
        ILogger<UserInterestsTagsProcessingHandlerWorker> logger)
        : base(nameof(UserInterestsTagsProcessingHandlerWorker), channel, logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteInternalAsync(
        UserInterestsProsessingMessage message,
        CancellationToken stoppingToken)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(message, nameof(message));
            using var scope = _services.CreateScope();
            var tagProcessorService = scope.ServiceProvider
                .GetRequiredService<TagProcessorService>();

            var results = await tagProcessorService
                .TranslateTagsAsync(message.Interests);

            var categorizerTags = results.ToList();
            if (categorizerTags.Count == 0)
                throw new RemoteServiceCallFailedException(
                    $"Error: Tag processor service returned empty result for user={message.UserProfileId}.");

            var dbContext = scope.ServiceProvider.GetRequiredService<ExpertBridgeDbContext>();

            var tags = categorizerTags
                .Select(result => new Tag
                    {
                        ArabicName = result.ArabicName,
                        EnglishName = result.EnglishName,
                        Description = result.Description
                    }).ToList();
            await dbContext.Tags.AddRangeAsync(tags, stoppingToken);
            await dbContext.UserInterests
                .AddRangeAsync(tags.Select(tag => new UserInterest
                    {
                        ProfileId = message.UserProfileId,
                        Tag = tag
                    }).ToList(), stoppingToken);
            await dbContext.SaveChangesAsync(stoppingToken);
        }
        catch (Exception ex)
        {
            // _logger.LogError(e,
            //     "Error processing user interests tags for user profile {UserProfileId}, Exception: {Exception}",
            //             message.UserProfileId,
            //             e.Message);
            Log.Error(ex,
                "Error processing user interests tags for user profile {UserProfileId}, Exception: {Exception}",
                        message.UserProfileId,
                        ex.Message);
        }
    }
}
