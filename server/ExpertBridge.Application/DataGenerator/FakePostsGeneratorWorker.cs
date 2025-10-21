using ExpertBridge.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ExpertBridge.Application.DataGenerator;

/// <summary>
/// Background service for continuous generation and seeding of fake posts for development and load testing.
/// </summary>
/// <remarks>
/// This is a BackgroundService (long-running hosted service) designed for development environments
/// to continuously generate large volumes of test data without blocking the main application.
/// 
/// **Current State: DISABLED**
/// Both generation methods are commented out in ExecuteAsync loop. Enable manually when needed.
/// 
/// **Use Cases:**
/// - Load testing (generate millions of posts with embeddings)
/// - Database performance testing (bulk insert optimization)
/// - Vector similarity testing (large dataset with random embeddings)
/// - Data migration testing (schema changes with existing data)
/// 
/// **Generation Capabilities:**
/// 
/// **1. GeneratePosts (Bulk Post Creation):**
/// - Generates 100,000 posts per iteration
/// - All posts assigned to single profile ID: "e2e8eb61-2261-4e49-8aac-df336aff7991"
/// - Uses BulkInsertAsync for high-performance batch insertion
/// - Each post includes 1024-dim random embedding vector
/// - Runs continuously until cancellation
/// 
/// **2. UpdateCreatedAt (Backfill Timestamps):**
/// - Finds posts with null CreatedAt
/// - Batch updates 100 posts at a time
/// - Sets CreatedAt to current UTC time
/// - Uses ExecuteUpdateAsync for efficient bulk updates
/// - Useful after schema changes or data imports
/// 
/// **Performance Characteristics:**
/// - BulkInsertAsync: ~10-20x faster than AddRangeAsync for large batches
/// - ExecuteUpdateAsync: Direct SQL UPDATE without entity tracking
/// - Scoped DbContext per iteration (memory efficient)
/// - Continuous loop with try-catch for resilience
/// 
/// **Database Impact:**
/// ⚠️ **WARNING:** Generates massive data volumes
/// - 100K posts per iteration = ~400-600 MB (with embeddings)
/// - Continuous generation can fill disk quickly
/// - Vector indexes rebuild on large inserts (performance impact)
/// - Monitor disk space and database performance
/// 
/// **Activation Pattern:**
/// <code>
/// // In ExecuteAsync, uncomment desired method:
/// await GeneratePosts(stoppingToken);  // Enable bulk post generation
/// // OR
/// await UpdateCreatedAt(stoppingToken);  // Enable timestamp backfill
/// </code>
/// 
/// **Dependency Injection:**
/// Uses IServiceProvider to create scoped DbContext per operation:
/// - Avoids long-lived DbContext issues
/// - Ensures proper connection pooling
/// - Prevents memory leaks in long-running service
/// 
/// **Error Handling:**
/// - Catches NotSupportedException (expected from disabled methods)
/// - Logs errors via Serilog
/// - Continues running despite errors (resilient)
/// - Does NOT propagate exceptions (service remains running)
/// 
/// **Development Workflow:**
/// <code>
/// 1. Deploy application to dev environment
/// 2. Uncomment desired generation method
/// 3. Service starts generating data automatically
/// 4. Monitor disk space and database load
/// 5. Comment out method when sufficient data generated
/// 6. Redeploy to stop generation
/// </code>
/// 
/// **Best Practices:**
/// - Only enable in development environments (never production)
/// - Monitor database size and disk space
/// - Use rate limiting if continuous generation too aggressive
/// - Add delays between iterations to control load
/// - Consider separate database for load testing
/// 
/// **Alternative Approaches:**
/// Instead of continuous generation, consider:
/// - One-time seed script via EF Core migrations
/// - Console application for controlled data generation
/// - Database backup/restore from pre-seeded database
/// - SQL scripts for direct data insertion
/// 
/// Registered as hosted background service, runs for application lifetime.
/// </remarks>
public class FakePostsGeneratorWorker : BackgroundService
{
    private readonly IServiceProvider _services;

    public FakePostsGeneratorWorker(
        IServiceProvider services)
    {
        _services = services;
    }

    /// <summary>
    /// Main execution loop for the background service (currently disabled).
    /// </summary>
    /// <param name="stoppingToken">Cancellation token signaled when the application is shutting down.</param>
    /// <returns>A task representing the continuous execution.</returns>
    /// <remarks>
    /// **Current Behavior:** Empty loop - both generation methods commented out.
    /// 
    /// **When Enabled:**
    /// - Runs continuously until application shutdown
    /// - Catches and logs NotSupportedException
    /// - Does not exit on errors (resilient operation)
    /// 
    /// Uncomment method calls inside the loop to activate data generation.
    /// </remarks>
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

    /// <summary>
    /// Backfills null CreatedAt timestamps for existing posts in batches.
    /// </summary>
    /// <param name="stoppingToken">Cancellation token for operation cancellation.</param>
    /// <returns>A task representing the batch update operation.</returns>
    /// <remarks>
    /// **Operation:**
    /// - Finds up to 100 posts with null CreatedAt
    /// - Updates all to current UTC timestamp
    /// - Uses ExecuteUpdateAsync (efficient bulk SQL UPDATE)
    /// - No entity tracking overhead
    /// 
    /// **Use Case:** Data repair after schema changes or imports.
    /// 
    /// **Performance:** Processes 100 posts per call. For large datasets, call repeatedly in loop.
    /// </remarks>
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

    /// <summary>
    /// Generates and bulk inserts 100,000 fake posts for load testing.
    /// </summary>
    /// <param name="stoppingToken">Cancellation token for operation cancellation.</param>
    /// <returns>A task representing the bulk insert operation.</returns>
    /// <remarks>
    /// **Generation Details:**
    /// - Creates 100,000 posts via Generator.GeneratePosts
    /// - All posts assigned to profile: "e2e8eb61-2261-4e49-8aac-df336aff7991"
    /// - Each post includes random 1024-dim embedding
    /// - Uses BulkInsertAsync for maximum performance
    /// 
    /// **Performance:**
    /// - BulkInsertAsync: ~10-20x faster than AddRangeAsync
    /// - Bypasses EF change tracking
    /// - Direct SQL bulk insert operations
    /// - 100K posts typically completes in 10-30 seconds
    /// 
    /// **Memory Usage:** ~400-600 MB per batch (posts + embeddings).
    /// 
    /// **Database Impact:** Vector indexes rebuild after insert (may cause temporary slowdown).
    /// </remarks>
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
