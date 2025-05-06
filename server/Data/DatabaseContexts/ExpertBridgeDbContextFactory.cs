

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Data.DatabaseContexts;

public class ExpertBridgeDbContextFactory : IDesignTimeDbContextFactory<ExpertBridgeDbContext>
{
    public ExpertBridgeDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables("ASPNETCORE_ENVIRONMENT")
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json")
            .AddUserSecrets<ExpertBridgeDbContext>()
            .Build();

        var connectionString = configuration.GetConnectionString("Postgresql")!;
        var optionsBuilder = new DbContextOptionsBuilder<ExpertBridgeDbContext>();
        optionsBuilder.UseNpgsql(connectionString, o =>
        {
            o.UseVector();
            o.EnableRetryOnFailure(
                maxRetryCount: 10,
                maxRetryDelay: TimeSpan.FromSeconds(60),
                errorCodesToAdd: null);
        });
        return new ExpertBridgeDbContext(optionsBuilder.Options);
    }
}
