// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.


using System.Threading.Channels;
using ExpertBridge.Api.Extensions;
using ExpertBridge.Api.HttpClients;
using ExpertBridge.Api.Models.IPC;
using ExpertBridge.Api.Requests;
using ExpertBridge.Api.Services;
using ExpertBridge.Core.Entities;
using ExpertBridge.Data.DatabaseContexts;

namespace ExpertBridge.Api.BackgroundServices
{
    public class UserInterestsUpdatedHandlerWorker : BackgroundService
    {
        private readonly ExpertBridgeDbContext _dbContext;
        private readonly ILogger<UserInterestsUpdatedHandlerWorker> _logger;
        private readonly ChannelReader<UserInterestsUpdatedMessage> _channel;

        public UserInterestsUpdatedHandlerWorker(
            ExpertBridgeDbContext dbContext,
            ILogger<UserInterestsUpdatedHandlerWorker> logger,
            Channel<UserInterestsUpdatedMessage> channel)
        {
            _dbContext = dbContext;
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
                        
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"An error occurred while processing " +
                            $"message with user profile id={message.UserProfileId}.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    @$"{nameof(UserInterestsUpdatedHandlerWorker)} ran into unexpected error: 
                    An error occurred while reading from the channel.");
            }
            finally
            {
                _logger.LogInformation($"Terminating {nameof(UserInterestsUpdatedHandlerWorker)}.");
            }
        }
    }
}
