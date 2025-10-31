using ExpertBridge.Host.Resources;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var rabbitMq = builder.GetRabbitMqResource();
var ollama = builder.GetOllamaResource();
var redis = builder.GetRedisResource();
var postgresql = builder.GetPostgresqlResource();
var expertBridgeDb = postgresql.AddDatabase("ExpertBridgeDb");
var quartzDb = postgresql.AddDatabase("QuartzDb");

builder.AddProject<ExpertBridge_Api>("ExpertBridgeApi")
    .WithReference(redis)
    .WithReference(rabbitMq)
    .WithReference(ollama)
    .WithReference(expertBridgeDb)
    .WaitFor(expertBridgeDb)
    .WaitFor(rabbitMq)
    .WaitFor(redis)
    .WaitFor(ollama)
    .WithOtlpExporter()
    .WithExternalHttpEndpoints();

builder.AddProject<ExpertBridge_Admin>("ExpertBridgeAdmin")
    .WithReference(redis)
    .WithReference(rabbitMq)
    .WithReference(expertBridgeDb)
    .WaitFor(expertBridgeDb)
    .WaitFor(rabbitMq)
    .WaitFor(redis)
    .WaitFor(rabbitMq)
    .WithOtlpExporter()
    .WithExternalHttpEndpoints();

builder.AddProject<ExpertBridge_Worker>("ExpertBridgeWorker")
    .WithReference(redis)
    .WithReference(rabbitMq)
    .WithReference(ollama)
    .WithReference(quartzDb)
    .WithReference(expertBridgeDb)
    .WaitFor(rabbitMq)
    .WaitFor(redis)
    .WaitFor(ollama)
    .WaitFor(quartzDb)
    .WaitFor(expertBridgeDb)
    .WithOtlpExporter();

await builder.Build().RunAsync();
