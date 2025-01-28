var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder
    .AddPostgres("Postgresql", port: 4000)
    .WithImage("postgres", "17-alpine")
    .WithContainerName("expertbridge-postgresql")
    .WithEnvironment("POSTGRES_USER", "root")
    .WithEnvironment("POSTGRES_PASSWORD", "root")
    .WithDataVolume("expertbridge-postgresql-data")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgWeb()
    .PublishAsConnectionString();

var db = postgres
    .AddDatabase("ExpertBridgeDb", "ExpertBridgeDb");

var redis = builder
    .AddRedis("Redis", port: 4001)
    .WithImage("redis", "alpine")
    .WithContainerName("expertbridge-redis")
    .WithDataVolume("expertbridge-redis-data")
    .WithPersistence(TimeSpan.FromMinutes(5))
    .WithLifetime(ContainerLifetime.Persistent);

builder.AddProject<Projects.ExpertBridge_Api>("ExpertBridgeApi")
    .WithReference(db)
    .WithReference(redis)
    .WaitFor(db)
    .WaitFor(redis)
    .WithExternalHttpEndpoints();

await builder.Build().RunAsync();
