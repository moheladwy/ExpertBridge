// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.HttpClients;
using ExpertBridge.Api.Settings;
using Refit;

namespace ExpertBridge.Api.Extensions;

/// <summary>
///     Provides functionality for registering Refit-based HTTP clients with dependency injection.
/// </summary>
public static class RefitHttpClients
{
    /// <summary>
    ///     Adds the Refit HTTP clients to the services collection.
    /// </summary>
    /// <param name="services">
    ///     The service collection to add the Refit HTTP clients to.
    /// </param>
    public static void AddRefitHttpClients(this WebApplicationBuilder builder)
    {
        var config = builder.Configuration.GetSection(PostCategorizerSettings.Section);
        builder.Services.Configure<PostCategorizerSettings>(config);

        var categorizerSettings = config.Get<PostCategorizerSettings>();

        builder.Services.AddRefitClient<IPostCategroizerClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(categorizerSettings?.BaseUrl ?? string.Empty));
    }
}
