using ExpertBridge.Application;
using ExpertBridge.Data;
using ExpertBridge.Extensions.AWS;
using ExpertBridge.Extensions.Caching;
using ExpertBridge.Extensions.Embeddings;
using ExpertBridge.Extensions.Logging;
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
builder
    .AddSerilogLogging()
    .ConfigureExpertBridgeSettings()
    .AddFusionCache()
    .ConfigureOpenTelemetry()
    .ConfigureHttpClientDefaults()
    .AddResiliencePipeline()
    .AddEmbeddingServices()
    .AddGroqApiServices();

builder.AddNotifications();
builder.Services.AddDomainServices();
var section = builder.Configuration.GetSection(AwsSettings.Section);
builder.Services.Configure<AwsSettings>(section);
builder.AddS3ObjectService();
builder.RegisterMessageBroker(typeof(UserInterestsUpdatedConsumer).Assembly);
builder.Services.AddBackgroundServices(builder.Configuration);

var host = builder.Build();

await host.ApplyMigrationAtStartup();

await host.RunAsync();
