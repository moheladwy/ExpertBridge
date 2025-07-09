using ExpertBridge.Application.Settings;
using Microsoft.Extensions.AI;

namespace ExpertBridge.Api.Extensions
{
    public static class Embedding
    {
        public static WebApplicationBuilder AddEmbeddingServices(this WebApplicationBuilder builder)
        {
            var config = builder.Configuration.GetSection(EmbeddingServiceSettings.Section);
            builder.Services.Configure<EmbeddingServiceSettings>(config);

            var settings = config.Get<EmbeddingServiceSettings>();

            builder.Services.AddSingleton<IEmbeddingGenerator<string, Embedding<float>>>(
                serviceProvider => new OllamaEmbeddingGenerator(
                    settings.Endpoint,
                    settings.ModelId)
            );

            return builder;
        }
    }
}
