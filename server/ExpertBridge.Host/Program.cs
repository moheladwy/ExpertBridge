using ExpertBridge.Host.Resources;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var rabbitMq = builder.GetRabbitMqResource();
var ollama = builder.GetOllamaResource();
var redis = builder.GetRedisResource();
var postgresql = builder.GetPostgresqlResource();
var expertBridgeDb = postgresql.AddDatabase("ExpertBridgeDb");
var quartzDb = postgresql.AddDatabase("QuartzDb");
var adminDb = postgresql.AddDatabase("AdminDb");

var api = builder
    .AddProject<ExpertBridge_Api>("api")
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

builder
    .AddProject<ExpertBridge_Admin>("admin")
    .WithReference(redis)
    .WithReference(rabbitMq)
    .WithReference(expertBridgeDb)
    .WithReference(adminDb)
    .WaitFor(expertBridgeDb)
    .WaitFor(rabbitMq)
    .WaitFor(redis)
    .WaitFor(rabbitMq)
    .WaitFor(adminDb)
    .WithOtlpExporter()
    .WithExternalHttpEndpoints();

builder
    .AddProject<ExpertBridge_Worker>("worker")
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

builder
    .AddViteApp("client", "../../client")
    .WaitFor(api)
    .WithEnvironments(builder)
    .WithOtlpExporter()
    .WithHttpsEndpoint(port: 5173);

await builder.Build().RunAsync();
