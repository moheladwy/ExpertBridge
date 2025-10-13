// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ExpertBridge.Extensions.Embeddings;

public static class Embedding
{
    public static TBuilder AddEmbeddingServices<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        var config = builder.Configuration.GetSection(EmbeddingServiceSettings.Section);
        builder.Services.Configure<EmbeddingServiceSettings>(config);

        var settings = config.Get<EmbeddingServiceSettings>();

        builder.Services.AddSingleton<IEmbeddingGenerator<string, Embedding<float>>>(
            serviceProvider =>
                new OllamaEmbeddingGenerator(
                    settings.Endpoint,
                    settings.ModelId)
        );

        return builder;
    }
}
