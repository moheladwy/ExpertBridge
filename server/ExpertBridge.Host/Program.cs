var builder = DistributedApplication.CreateBuilder(args);

var redis = builder
    .AddRedis("Redis", port: 4001)
    .WithImage("redis", "alpine")
    .WithContainerName("expertbridge-redis")
    .WithDataVolume("expertbridge-redis-data")
    .WithPersistence(TimeSpan.FromMinutes(5))
    .WithLifetime(ContainerLifetime.Persistent)
    .PublishAsConnectionString();

builder.AddProject<Projects.ExpertBridge_Api>("ExpertBridgeApi")
    .WithReference(redis)
    .WaitFor(redis)
    .WithExternalHttpEndpoints();

builder.Build().Run();
