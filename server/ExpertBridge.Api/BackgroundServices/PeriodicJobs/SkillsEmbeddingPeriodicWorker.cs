using System.Threading.Channels;
using ExpertBridge.Api.Models;
using ExpertBridge.Api.Models.IPC;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Api.BackgroundServices.PeriodicJobs;

public class SkillsEmbeddingPeriodicWorker
    : PeriodicWorker<SkillsEmbeddingPeriodicWorker>
{
    private readonly IServiceProvider _services;
    private readonly ChannelWriter<EmbedSkillsMessage> _channel;

    public SkillsEmbeddingPeriodicWorker(
        ILogger<SkillsEmbeddingPeriodicWorker> logger,
        IServiceProvider services,
        Channel<EmbedSkillsMessage> channel)
        : base(PeriodicJobsStartDelays.ProfileSkillsEmbeddingPeriodicWorkerStartDelay,
            nameof(SkillsEmbeddingPeriodicWorker),
            logger)
    {
        _services = services;
        _channel = channel.Writer;
    }

    protected override async Task ExecuteInternalAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ExpertBridgeDbContext>();

            await dbContext.Skills
                .AsNoTracking()
                .Where(s => s.Description != null && s.Embedding == null)
                .Select(s => new EmbedSkillsMessage
                {
                    SkillId = s.Id
                })
                .ForEachAsync(async void (skill) =>
                    await _channel.WriteAsync(skill, stoppingToken),
                    stoppingToken
                );
        }
        catch (Exception e)
        {
            _logger.LogError("Error while executing {WorkerName}: {Message}",
                nameof(SkillsEmbeddingPeriodicWorker), e.Message);
        }
    }
}
