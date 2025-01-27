using ExpertBridge.Api;
using ExpertBridge.Api.DatabaseConfigurations;
using ExpertBridge.Api.Middlewares;
using ExpertBridge.Data.DatabaseContexts;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.AddSqlServerDbContext<ExpertBridgeDbContext>(connectionName: "SqlServer");
builder.AddRedisDistributedCache(connectionName: "Redis");
builder.Services.AddHealthChecks();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.AddLoggingService();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    await app.UseAutoMigrationAtStartup();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

await app.RunAsync();
