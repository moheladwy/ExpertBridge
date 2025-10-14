using ExpertBridge.Application.EmbeddingService;
using ExpertBridge.Application.Services;
using ExpertBridge.Data;
using ExpertBridge.Extensions.AWS;
using ExpertBridge.Extensions.Embeddings;
using ExpertBridge.Extensions.MessageBroker;
using ExpertBridge.GroqLibrary;
using ExpertBridge.Notifications.Extensions;
using ExpertBridge.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddBackgroundServices(builder.Configuration);
builder.Services.AddNotifications();
builder
    .AddEmbeddingServices()
    .AddGroqApiServices();
builder.Services.AddSingleton<IEmbeddingService, OllamaEmbeddingService>();
builder.Services
    .AddScoped<GroqPostTaggingService>()
    .AddScoped<GroqTagProcessorService>()
    .AddScoped<GroqInappropriateLanguageDetectionService>()
    ;
builder.AddS3ObjectService();
builder.RegisterMessageBroker(typeof(Program).Assembly);

var host = builder.Build();

host.Run();
