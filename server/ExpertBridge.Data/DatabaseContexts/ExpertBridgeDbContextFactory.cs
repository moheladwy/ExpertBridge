using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ExpertBridge.Data.DatabaseContexts;

public class ExpertBridgeDbContextFactory : IDesignTimeDbContextFactory<ExpertBridgeDbContext>
{
    public ExpertBridgeDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables("ASPNETCORE_ENVIRONMENT")
            .AddJsonFile("appsettings.Development.json")
            .AddJsonFile("appsettings.json")
            .AddUserSecrets<ExpertBridgeDbContext>()
            .Build();

        string connectionString = configuration.GetConnectionString("ExpertBridgeDb")!;
        var builder = new DbContextOptionsBuilder<ExpertBridgeDbContext>();
        builder.UseSqlServer(connectionString);
        return new ExpertBridgeDbContext(builder.Options);
    }
}
