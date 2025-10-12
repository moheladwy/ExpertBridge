using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var rabbitMqUsername = builder
    .AddParameterFromConfiguration("RabbitMq-Username", "RabbitMQ:Username");
var rabbitMqPassword = builder
    .AddParameterFromConfiguration("RabbitMq-Password", "RabbitMQ:Password");

var rabbitMq = builder
    .AddRabbitMQ("rabbitmq", rabbitMqUsername, rabbitMqPassword, port: 5672)
    .WithContainerName("expertbridge-rabbitmq")
    .WithDataVolume("expertbridge-rabbitmq-data")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithManagementPlugin()
    .WithOtlpExporter()
    .WithExternalHttpEndpoints();

var redis = builder
    .AddRedis("Redis", port: 6379)
    .WithImage("redis", "alpine")
    .WithContainerName("expertbridge-redis")
    .WithDataVolume("expertbridge-redis-data")
    .WithPersistence(TimeSpan.FromMinutes(5))
    .WithLifetime(ContainerLifetime.Persistent)
    .WithOtlpExporter()
    .PublishAsConnectionString();

var seq = builder
    .AddSeq("Seq", port: 4002)
    .WithContainerName("expertbridge-seq")
    .WithDataVolume("expertbridge-seq-data")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithOtlpExporter()
    .WithExternalHttpEndpoints();

builder.AddProject<ExpertBridge_Api>("ExpertBridgeApi")
    .WithReference(redis)
    .WithReference(seq)
    .WaitFor(rabbitMq)
    .WaitFor(redis)
    .WaitFor(seq)
    .WaitFor(rabbitMq)
    .WithOtlpExporter()
    .WithExternalHttpEndpoints();

builder.AddProject<ExpertBridge_Admin>("ExpertBridgeAdmin")
    .WithReference(redis)
    .WithReference(seq)
    .WaitFor(rabbitMq)
    .WaitFor(seq)
    .WaitFor(redis)
    .WaitFor(rabbitMq)
    .WithOtlpExporter()
    .WithExternalHttpEndpoints();

await builder.Build().RunAsync();
