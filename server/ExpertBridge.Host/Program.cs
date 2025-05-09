using Projects;

var builder = DistributedApplication.CreateBuilder(args);
var redisPassword = builder.Configuration["Redis:REDIS_PASSWORD"]!;

var redis = builder
    .AddRedis("Redis", port: 6379)
    .WithImage("redis", "alpine")
    .WithContainerName("expertbridge-redis")
    .WithDataVolume("expertbridge-redis-data")
    .WithPersistence(TimeSpan.FromMinutes(5))
    .WithLifetime(ContainerLifetime.Persistent)
    .WithOtlpExporter()
    .PublishAsConnectionString();

var seq = builder.AddSeq("Seq", port: 4002)
    .WithContainerName("expertbridge-seq")
    .WithDataVolume("expertbridge-seq-data")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithOtlpExporter()
    .WithExternalHttpEndpoints();

builder.AddProject<ExpertBridge_Api>("ExpertBridgeApi")
    .WithReference(redis)
    .WithReference(seq)
    .WaitFor(redis)
    .WaitFor(seq)
    .WithOtlpExporter()
    .WithExternalHttpEndpoints();

builder.AddProject<ExpertBridge_Admin>("ExpertBridgeAdmin")
    .WithReference(redis)
    .WithReference(seq)
    .WaitFor(seq)
    .WaitFor(redis)
    .WithOtlpExporter()
    .WithExternalHttpEndpoints();

builder.Build().Run();
