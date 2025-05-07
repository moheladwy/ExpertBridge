using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExpertBridge.Extensions.Embeddings;

public static class Embedding
{
    public static WebApplicationBuilder AddEmbeddingServices(this WebApplicationBuilder builder)
    {
        var config = builder.Configuration.GetSection(EmbeddingServiceSettings.Section);
        builder.Services.Configure<EmbeddingServiceSettings>(config);

        var settings = config.Get<EmbeddingServiceSettings>();

        builder.Services.AddSingleton<IEmbeddingGenerator<string, Embedding<float>>>(serviceProvider =>
            new OllamaEmbeddingGenerator(
                settings.Endpoint,
                settings.ModelId)
        );

        return builder;
    }
}
