// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.


using ExpertBridge.Data.DatabaseContexts;
using Serilog;

namespace ExpertBridge.Api.DataGenerator
{
    public class FakePostsGeneratorWorker : BackgroundService
    {
        private readonly IServiceProvider _services;

        public FakePostsGeneratorWorker(
            IServiceProvider services)
        {
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var posts = Generator.GeneratePosts("e2e8eb61-2261-4e49-8aac-df336aff7991", 100000);

                    using var scope = _services.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ExpertBridgeDbContext>();

                    //await dbContext.Posts.AddRangeAsync(posts, stoppingToken);
                    //await dbContext.SaveChangesAsync(stoppingToken);

                    await dbContext.BulkInsertAsync(posts);
                }
                catch (NotSupportedException ex)
                {
                    Log.Error(ex, "Error occurred while generating fake posts.");
                }
            }
        }
    }
}
