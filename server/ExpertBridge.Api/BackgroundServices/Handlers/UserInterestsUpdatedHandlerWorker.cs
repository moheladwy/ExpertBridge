// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.


using System.Text;
using System.Threading.Channels;
using ExpertBridge.Core;
using ExpertBridge.Data.DatabaseContexts;
using ExpertBridge.Api.EmbeddingService;
using ExpertBridge.Api.Models.IPC;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ExpertBridge.Api.BackgroundServices.Handlers
{
    public class UserInterestsUpdatedHandlerWorker : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<UserInterestsUpdatedHandlerWorker> _logger;
        private readonly ChannelReader<UserInterestsUpdatedMessage> _channel;

        public UserInterestsUpdatedHandlerWorker(
            IServiceProvider services,
            ILogger<UserInterestsUpdatedHandlerWorker> logger,
            Channel<UserInterestsUpdatedMessage> channel)
        {
            _services = services;
            _logger = logger;
            _channel = channel.Reader;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (await _channel.WaitToReadAsync(stoppingToken))
                {
                    var message = await _channel.ReadAsync(stoppingToken);

                    try
                    {
                        using var scope = _services.CreateScope();
                        var dbContext = scope.ServiceProvider.GetRequiredService<ExpertBridgeDbContext>();
                        var embeddingService = scope.ServiceProvider.GetRequiredService<IEmbeddingService>();

                        var userInterests = dbContext.UserInterests
                            .AsNoTracking()
                            .Include(ui => ui.Tag)
                            .Where(ui => ui.ProfileId == message.UserProfileId)
                            .Select(ui => $"{ui.Tag.EnglishName} {ui.Tag.ArabicName} ");

                        var text = new StringBuilder();
                        foreach (var userInterest in userInterests)
                        {
                            text.Append(userInterest);
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
            catch (Exception ex)
            {
                // _logger.LogError(ex,
                //     @$"{nameof(UserInterestsUpdatedHandlerWorker)} ran into unexpected error:
                //     An error occurred while reading from the channel.");
                Log.Error(ex,
                    "An error occurred while reading from the channel in " +
                    "{nameof(UserInterestsUpdatedHandlerWorker)}.",
                    nameof(UserInterestsUpdatedHandlerWorker));
            }
            finally
            {
                // _logger.LogInformation($"Terminating {nameof(UserInterestsUpdatedHandlerWorker)}.");
                Log.Information("Terminating {nameof(UserInterestsUpdatedHandlerWorker)}.",
                    nameof(UserInterestsUpdatedHandlerWorker));
            }
        }
    }
}
