using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var rabbitMqUsername = builder
    .AddParameterFromConfiguration("RabbitMq-Username", "RabbitMQ:Username");
var rabbitMqPassword = builder
    .AddParameterFromConfiguration("RabbitMq-Password", "RabbitMQ:Password");

var rabbitMq = builder
    .AddRabbitMQ("rabbitmq", rabbitMqUsername, rabbitMqPassword, 5672)
    .WithContainerName("expertbridge-rabbitmq")
    .WithDataVolume("expertbridge-rabbitmq-data")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithManagementPlugin()
    .WithOtlpExporter()
    .WithExternalHttpEndpoints();

var ollama = builder
    .AddOllama("ollama", 11434)
    .WithContainerName("expertbridge-ollama")
    .WithDataVolume("expertbridge-ollama-data")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithGPUSupport()
    // .WithOpenWebUI()
    .WithOtlpExporter()
    .PublishAsConnectionString()
    .AddModel("snowflake-arctic-embed2:latest");

var redis = builder
    .AddRedis("Redis", 6379)
    .WithImage("redis", "alpine")
    .WithContainerName("expertbridge-redis")
    .WithDataVolume("expertbridge-redis-data")
    .WithPersistence(TimeSpan.FromMinutes(5))
    .WithLifetime(ContainerLifetime.Persistent)
    .WithOtlpExporter()
    .PublishAsConnectionString();

var seq = builder
    .AddSeq("Seq", 4002)
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

builder.AddProject<ExpertBridge_Worker>("ExpertBridgeWorker")
    .WithReference(redis)
    .WithReference(rabbitMq)
    .WithReference(seq)
    .WithReference(ollama)
    .WaitFor(rabbitMq)
    .WaitFor(redis)
    .WaitFor(seq)
    .WaitFor(ollama)
    .WithOtlpExporter();

await builder.Build().RunAsync();
