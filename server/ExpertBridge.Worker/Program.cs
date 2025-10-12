using ExpertBridge.Extensions.MessageBroker;

var builder = Host.CreateApplicationBuilder(args);

builder.RegisterMessageBroker(typeof(Program).Assembly);

var host = builder.Build();

host.Run();
