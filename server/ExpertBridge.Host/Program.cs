var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder
    .AddPostgres("Postgresql", port: 4000)
    .WithImage("postgres", "17-alpine")
    .WithContainerName("expertbridge-postgresql")
    .WithDataVolume("expertbridge-postgresql-data")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgWeb()
    .PublishAsConnectionString();

var expertBridgeDb = postgres
    .AddDatabase("ExpertBridgeDb", "ExpertBridgeDb");

var keycloakDb = postgres
    .AddDatabase("KeycloakDb", "KeycloakDb");

var keycloak = builder.AddKeycloak("Keycloak", port: 4002)
    .WithContainerName("expertbridge-keycloak")
    .WithReference(keycloakDb)
    .WaitFor(keycloakDb)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithExternalHttpEndpoints();

var redis = builder
    .AddRedis("Redis", port: 4001)
    .WithImage("redis", "alpine")
    .WithContainerName("expertbridge-redis")
    .WithDataVolume("expertbridge-redis-data")
    .WithPersistence(TimeSpan.FromMinutes(5))
    .WithLifetime(ContainerLifetime.Persistent);

builder.AddProject<Projects.ExpertBridge_Api>("ExpertBridgeApi")
    .WithReference(expertBridgeDb)
    .WithReference(keycloak)
    .WithReference(redis)
    .WaitFor(expertBridgeDb)
    .WaitFor(keycloak)
    .WaitFor(redis)
    .WithExternalHttpEndpoints();

await builder.Build().RunAsync();
