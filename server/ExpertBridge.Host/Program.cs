var builder = DistributedApplication.CreateBuilder(args);

var mssql = builder
    .AddSqlServer("SqlServer", port: 4000)
    .WithImage("mssql/server", "2022-latest")
    .WithContainerName("expertbridge-sqlserver")
    .WithDataVolume("expertbridge-sqlserver-data")
    .WithEnvironment("MSSQL_SA_PASSWORD", "Password123")
    .WithEnvironment("ACCEPT_EULA", "Y")
    .WithEnvironment("MSSQL_PID", "Developer")
    .WithLifetime(ContainerLifetime.Persistent)
    .PublishAsConnectionString();

var db = mssql
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
