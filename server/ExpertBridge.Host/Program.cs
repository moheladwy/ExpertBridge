using ExpertBridge.Host.Resources;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var rabbitMq = builder.GetRabbitMqResource();
var ollama = builder.GetOllamaResource();
var redis = builder.GetRedisResource();
var postgresql = builder.GetPostgresqlResource();

builder.AddProject<ExpertBridge_Api>("ExpertBridgeApi")
    .WithReference(redis)
    .WithReference(postgresql)
    .WithReference(rabbitMq)
    .WithReference(ollama)
    .WaitFor(rabbitMq)
    .WaitFor(redis)
    .WaitFor(rabbitMq)
    .WaitFor(ollama)
    .WithOtlpExporter()
    .WithExternalHttpEndpoints();

builder.AddProject<ExpertBridge_Admin>("ExpertBridgeAdmin")
    .WithReference(redis)
    .WithReference(postgresql)
    .WithReference(rabbitMq)
    .WaitFor(rabbitMq)
    .WaitFor(redis)
    .WaitFor(rabbitMq)
    .WithOtlpExporter()
    .WithExternalHttpEndpoints();

builder.AddProject<ExpertBridge_Worker>("ExpertBridgeWorker")
    .WithReference(redis)
    .WithReference(rabbitMq)
    .WithReference(ollama)
    .WithReference(postgresql)
    .WaitFor(rabbitMq)
    .WaitFor(redis)
    .WaitFor(ollama)
    .WaitFor(postgresql)
    .WithOtlpExporter();

await builder.Build().RunAsync();
