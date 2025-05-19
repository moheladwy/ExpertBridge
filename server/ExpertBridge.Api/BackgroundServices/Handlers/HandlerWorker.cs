// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Models.IPC;
using ExpertBridge.Api.Services;
using Serilog;
using System.Threading.Channels;

namespace ExpertBridge.Api.BackgroundServices.Handlers
{
    public abstract class HandlerWorker<TWorker, TMessage> : BackgroundService where TMessage : class
    {
        private readonly string _workerName;
        private readonly ChannelReader<TMessage> _channel;
        private readonly ILogger<TWorker> _logger;

        public HandlerWorker(
            string workerName,
            ChannelReader<TMessage> channel,
            ILogger<TWorker> logger)
        {
            _workerName = workerName;
            _channel = channel;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (await _channel.WaitToReadAsync(stoppingToken))
                {
                    var message = await _channel.ReadAsync(stoppingToken);

                    Log.Information("Worker {workerName} received message: {message}", _workerName, nameof(message));
                    Log.Information("Start processing the message in {workerName}", _workerName);
                    await ExecuteInternalAsync(message, stoppingToken);
                    Log.Information("Finished processing message in {workerName}", _workerName);
                }
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex,
                //     @$"{_workerName} ran into unexpected error:
                //     An error occurred while reading from the channel.");
                Log.Error(ex,
                    "An error occurred while reading from the channel in {0}.", _workerName);
            }
            finally
            {
                // _logger.LogInformation($"Terminating {_workerName}.");
                Log.Information("Terminating {0}.", _workerName);
            }
        }

        protected abstract Task ExecuteInternalAsync(TMessage message, CancellationToken stoppingToken);
    }
}
