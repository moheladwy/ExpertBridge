// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Settings;
using ExpertBridge.Api.Settings.Serilog;
using ExpertBridge.Api.Data;
using ExpertBridge.Api.Extensions;
using ExpertBridge.Api.Middleware;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.ConfigureOpenTelemetry();

builder.AddDefaultHealthChecks();
builder.Services.AddServiceDiscovery();
builder.Services.ConfigureHttpClientDefaults(http =>
{
    http.AddStandardResilienceHandler();
    http.AddServiceDiscovery();
});

builder.Services.Configure<ConnectionStrings>(
    builder.Configuration.GetSection("ConnectionStrings"));

builder.Services.Configure<FirebaseSettings>(
    builder.Configuration.GetSection("Firebase"));

builder.Services.Configure<FirebaseAuthSettings>(
    builder.Configuration.GetSection("Authentication:Firebase"));

builder.Services.Configure<AwsSettings>(
    builder.Configuration.GetSection("AwsS3"));

builder.Services.Configure<AiSettings>(
    builder.Configuration.GetSection("AI"));

builder.Services.Configure<SerilogSettings>(
    builder.Configuration.GetSection("Serilog"));

builder.Services.AddDatabase(builder.Configuration);

builder.AddSeqEndpoint(connectionName: "Seq");
builder.AddRedisDistributedCache(connectionName: "Redis");
builder.AddS3ObjectService();

builder.AddFirebaseApp();
builder.AddFirebaseAuthentication();
builder.AddHttpClientForFirebaseService();

builder.AddSwaggerGen();
builder.AddCors();

builder.Services.AddControllers();
builder.Services.AddServices();
builder.Services.AddRepositories();

var app = builder.Build();

app.UseCors("AllowAll");
//app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    await app.ApplyMigrationAtStartup();
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapPrometheusScrapingEndpoint();
app.MapHealthChecks("/health", new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });
// Only health checks tagged with the "live" tag must pass for app to be considered alive
app.MapHealthChecks("/alive", new HealthCheckOptions { Predicate = r => r.Tags.Contains("live") });

await app.RunAsync();

// ReSharper disable once ClassNeverInstantiated.Global
namespace ExpertBridge.Api
{
    public partial class Program { }
}
