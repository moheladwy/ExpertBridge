using System.Threading.Channels;
using ExpertBridge.Api.Models.IPC;
using ExpertBridge.Data.DatabaseContexts;

namespace ExpertBridge.Api.BackgroundServices.Handlers;

public class SkillsTaggingHandlerWorker :
    HandlerWorker<SkillsTaggingHandlerWorker, TagSkillsMessage>
{
    private readonly IServiceProvider _services;

    public SkillsTaggingHandlerWorker(
        Channel<TagSkillsMessage> channel,
        ILogger<SkillsTaggingHandlerWorker> logger,
        IServiceProvider services)
        : base(nameof(SkillsTaggingHandlerWorker), channel.Reader, logger)
    {
        _services = services;
    }

    protected override Task ExecuteInternalAsync(TagSkillsMessage message, CancellationToken stoppingToken)
    {
        // using var scope = _services.CreateScope();
        // var dbContext = scope.ServiceProvider.GetRequiredService<ExpertBridgeDbContext>();

        throw new NotImplementedException();
    }
}
