using ExpertBridge.Api;
using ExpertBridge.Api.Extensions;
using ExpertBridge.Api.Middlewares;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));
builder.AddSeqEndpoint("Seq");
builder.AddServiceDefaults();
builder.AddDatabase();
builder.AddRedisDistributedCache(connectionName: "Redis");
builder.Services.AddControllers();
builder.AddAuthentication();
builder.Services.AddAuthorization();
builder.AddSwaggerGen();
builder.AddFirebaseServices();
builder.AddS3ObjectService();
builder.Services.AddServices();
builder.Services.AddRepositories();
builder.Services.AddCachedRepositories();
builder.AddHttpClientForFirebaseService();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    await app.ApplyMigrationAtStartup();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapPrometheusScrapingEndpoint();
app.MapHealthChecks("/health", new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });
// Only health checks tagged with the "live" tag must pass for app to be considered alive
app.MapHealthChecks("/alive", new HealthCheckOptions { Predicate = r => r.Tags.Contains("live") });

await app.RunAsync();
