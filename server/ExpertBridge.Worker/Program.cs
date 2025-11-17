using ExpertBridge.Application;
using ExpertBridge.Data;
using ExpertBridge.Extensions.AWS;
using ExpertBridge.Extensions.Caching;
using ExpertBridge.Extensions.Embeddings;
using ExpertBridge.Extensions.Logging;
using ExpertBridge.Extensions.MessageBroker;
using ExpertBridge.Extensions.OpenTelemetry;
using ExpertBridge.Extensions.Resilience;
using ExpertBridge.Notifications.Extensions;
using ExpertBridge.Worker;
using ExpertBridge.Worker.Consumers;
using ExpertBridge.Worker.QuartzDatabase;
using ExpertBridge.Worker.Services;
using Groq.Core.Models;
using Groq.Extensions.DependencyInjection;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddDatabase(builder.Configuration)
    .AddQuartzDbContext(builder.Configuration);
builder
    .AddSerilogLogging()
    .ConfigureExpertBridgeSettings()
    .AddFusionCache()
    .ConfigureOpenTelemetry()
    .ConfigureHttpClientDefaults()
    .AddResiliencePipeline()
    .AddEmbeddingServices()
    .AddGroqApiServices(options =>
    {
        options.ApiKey = builder.Configuration.GetValue<string>("Groq:ApiKey")!;
        options.Model = ChatModels.OPENAI_GPT_OSS_120B.Id;
        options.Timeout = TimeSpan.FromSeconds(15);
    });

builder.AddNotifications();
builder.Services
    .AddDomainServices()
    .AddScoped<AiTagProcessorService>()
    .AddScoped<AiPostTaggingService>()
    .AddScoped<NsfwContentDetectionService>()
    ;
var section = builder.Configuration.GetSection(AwsSettings.Section);
builder.Services.Configure<AwsSettings>(section);
builder.AddS3ObjectService();
builder.RegisterMessageBroker(typeof(UserInterestsUpdatedConsumer).Assembly);
builder.Services.AddBackgroundServices(builder.Configuration);

var host = builder.Build();

await host.ApplyMigrationAtStartup();

await host.RunAsync();
