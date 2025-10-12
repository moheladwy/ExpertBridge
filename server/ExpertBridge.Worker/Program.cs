using ExpertBridge.Data;
using ExpertBridge.Extensions.MessageBroker;
using ExpertBridge.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddBackgroundServices(builder.Configuration);
builder.RegisterMessageBroker(typeof(Program).Assembly);

var host = builder.Build();

host.Run();
