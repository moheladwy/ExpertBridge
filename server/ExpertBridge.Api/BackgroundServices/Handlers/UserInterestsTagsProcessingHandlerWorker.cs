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
    private readonly ChannelWriter<UserInterestsUpdatedMessage> _userInterestsUpdateChannelWriter;

    public UserInterestsTagsProcessingHandlerWorker(
        IServiceProvider services,
        Channel<UserInterestsProsessingMessage> channel,
        ILogger<UserInterestsTagsProcessingHandlerWorker> logger,
        Channel<UserInterestsUpdatedMessage> userInterestsUpdatedChannel
        )
        : base(nameof(UserInterestsTagsProcessingHandlerWorker), channel.Reader, logger)
    {
        _services = services;
        _logger = logger;
        _userInterestsUpdateChannelWriter = userInterestsUpdatedChannel.Writer;
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
                .GetRequiredService<GroqTagProcessorService>();

            var results = await tagProcessorService
                .TranslateTagsAsync(message.InterestsTags);

            if (results == null)
                throw new RemoteServiceCallFailedException(
                    $"Error: Tag processor service returned null result for user={message.UserProfileId}.");

            var categorizerTags = results.Tags;
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

            await _userInterestsUpdateChannelWriter.WriteAsync(new UserInterestsUpdatedMessage
                { UserProfileId = message.UserProfileId }, stoppingToken);
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
