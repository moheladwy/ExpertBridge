// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

var builder = DistributedApplication.CreateBuilder(args);

var redis = builder
    .AddRedis("Redis", port: 4001)
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
    .PublishAsConnectionString();

builder.AddProject<Projects.ExpertBridge_Api>("ExpertBridgeApi")
    .WithReference(redis)
    .WithReference(seq)
    .WaitFor(redis)
    .WaitFor(seq)
    .WithOtlpExporter()
    .WithExternalHttpEndpoints();

builder.Build().Run();
