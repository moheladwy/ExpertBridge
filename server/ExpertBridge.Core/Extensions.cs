using ExpertBridge.Core.Interfaces;
using ExpertBridge.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ExpertBridge.Core;

public static class Extensions
{
    /// <summary>
    ///     Adds the services to the services collection.
    /// </summary>
    /// <param name="services">
    ///     The service collection to add the services to.
    /// </param>
    public static void AddServices(this IServiceCollection services)
    {
        services.AddTransient<IFirebaseService, FirebaseService>();
    }
}
