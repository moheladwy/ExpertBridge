using System.Threading.Channels;
using ExpertBridge.Api.EmbeddingService;
using ExpertBridge.Api.Models.IPC;
using ExpertBridge.Core.Exceptions;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Api.BackgroundServices.Handlers;

public class SkillsEmbeddingHandlerWorker :
    HandlerWorker<SkillsEmbeddingHandlerWorker, EmbedSkillsMessage>
{
    private readonly IServiceProvider _services;
    private readonly ILogger<SkillsEmbeddingHandlerWorker> _logger;

    public SkillsEmbeddingHandlerWorker(
        IServiceProvider services,
        Channel<EmbedSkillsMessage> channel,
        ILogger<SkillsEmbeddingHandlerWorker> logger)
        : base(nameof(SkillsEmbeddingHandlerWorker), channel.Reader, logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteInternalAsync(EmbedSkillsMessage message, CancellationToken stoppingToken)
    {
        using var scope = _services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ExpertBridgeDbContext>();

        var skill = await dbContext.Skills
            .Where(s => s.Id == message.SkillId &&
                         s.Description != null &&
                         s.Embedding == null)
            .SingleOrDefaultAsync(stoppingToken);

        if (skill == null)
        {
            _logger.LogWarning("No skills found with Id={Id}", message.SkillId);
            return;
        }

        var embeddingService = _services.GetRequiredService<IEmbeddingService>();

        var embeddings = await embeddingService.GenerateEmbedding($"{skill.Name} {skill.Description}");
        if (embeddings is null)
        {
            _logger.LogError("Embedding service returned null for Skill with Id={Id}", message.SkillId);
            throw new RemoteServiceCallFailedException(
                $"Error: Embedding service returned null embedding for Skill with Id=${message.SkillId}.");
        }

        skill.Embedding = embeddings;
        await dbContext.SaveChangesAsync(stoppingToken);
    }
}
