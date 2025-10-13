using ExpertBridge.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ExpertBridge.Application.DataGenerator;

public class FakePostsGeneratorWorker : BackgroundService
{
    private readonly IServiceProvider _services;

    public FakePostsGeneratorWorker(
        IServiceProvider services) =>
        _services = services;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                //await GeneratePosts(stoppingToken);
                //await UpdateCreatedAt(stoppingToken);
            }
            catch (NotSupportedException ex)
            {
                Log.Error(ex, "Error occurred while generating fake posts.");
            }
        }
    }

    private async Task UpdateCreatedAt(CancellationToken stoppingToken)
    {
        using var scope = _services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ExpertBridgeDbContext>();

        await dbContext.Posts
            .Where(p => p.CreatedAt == null)
            .Take(100)
            .ExecuteUpdateAsync(setters => setters.SetProperty(p => p.CreatedAt, DateTime.UtcNow)
                , stoppingToken);
    }

    private async Task GeneratePosts(CancellationToken stoppingToken)
    {
        var posts = Generator.GeneratePosts("e2e8eb61-2261-4e49-8aac-df336aff7991", 100000);

        using var scope = _services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ExpertBridgeDbContext>();

        //await dbContext.Posts.AddRangeAsync(posts, stoppingToken);
        //await dbContext.SaveChangesAsync(stoppingToken);

        await dbContext.BulkInsertAsync(posts, stoppingToken);
    }
}
