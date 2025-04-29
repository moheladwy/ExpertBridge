

using ExpertBridge.Data.DatabaseContexts;
using ExpertBridge.Data.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExpertBridge.Data;

public static class Extensions
{
    /// <summary>
    ///     Adds the database service to the application builder.
    /// </summary>
    /// <param name="services">
    ///     The service collection to add the database service to.
    /// </param>
    /// <param name="configuration">
    ///     The configuration to get the connection string from.
    /// </param>
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgresql")!;
        services.AddDbContext<ExpertBridgeDbContext>(options =>
        {
            options.UseNpgsql(connectionString, o =>
            {
                o.UseVector();
            });
            options.AddInterceptors(new SoftDeleteInterceptor());
        });
        return services;
    }
}
