using ExpertBridge.Application.EmbeddingService;
using ExpertBridge.Application.Services;
using ExpertBridge.Data;
using ExpertBridge.Extensions.AWS;
using ExpertBridge.Extensions.Caching;
using ExpertBridge.Extensions.Embeddings;
using ExpertBridge.Extensions.MessageBroker;
using ExpertBridge.Extensions.OpenTelemetry;
using ExpertBridge.Extensions.Resilience;
using ExpertBridge.GroqLibrary;
using ExpertBridge.Notifications.Extensions;
using ExpertBridge.Worker;
using ExpertBridge.Worker.Consumers;
using ExpertBridge.Worker.QuartzDatabase;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddDatabase(builder.Configuration)
    .AddQuartzDbContext(builder.Configuration);
builder.AddFusionCache();
builder.ConfigureOpenTelemetry();
builder.ConfigureHttpClientDefaults();
builder.AddResiliencePipeline();
builder.Services.AddNotifications();
builder
    .AddEmbeddingServices()
    .AddGroqApiServices();
builder.Services
    .AddScoped<IEmbeddingService, OllamaEmbeddingService>()
    .AddScoped<GroqPostTaggingService>()
    .AddScoped<GroqTagProcessorService>()
    .AddScoped<GroqInappropriateLanguageDetectionService>()
    ;
builder.Services.Configure<AwsSettings>(
    builder.Configuration.GetSection(AwsSettings.Section));
builder.AddS3ObjectService();
builder.RegisterMessageBroker(typeof(UserInterestsUpdatedConsumer).Assembly);
builder.Services.AddBackgroundServices(builder.Configuration);

var host = builder.Build();

await host.ApplyMigrationAtStartup();

await host.RunAsync();
