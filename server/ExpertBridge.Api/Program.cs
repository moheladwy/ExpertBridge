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
using System.Security.Claims;
using ExpertBridge.Api.Models;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

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

builder.Services.Configure<ExpertBridgeRateLimitSettings>(
    builder.Configuration.GetSection(ExpertBridgeRateLimitSettings.SectionName));

builder.Services.AddDatabase(builder.Configuration);

builder.AddSeqEndpoint(connectionName: "Seq");
builder.AddRedisDistributedCache(connectionName: "Redis");
builder.AddS3ObjectService();

builder.AddFirebaseApp();
builder.AddFirebaseAuthentication();
builder.AddHttpClientForFirebaseService();

builder.AddSwaggerGen();
builder.AddCors();

builder.Services.AddResponseCaching();
builder.Services.AddControllers(options =>
{
    options.CacheProfiles.Add(CacheProfiles.Default,
        new CacheProfile
        {
            Duration = 300,
            Location = ResponseCacheLocation.Any
        });

    options.CacheProfiles.Add(CacheProfiles.PersonalizedContent,
        new CacheProfile
        {
            Duration = 180,
            Location = ResponseCacheLocation.Any,
            VaryByHeader = "Authorization",
        });
});

builder.Services.AddServices();
builder.Services.AddRepositories();

var rateLimitOptions = new ExpertBridgeRateLimitSettings();
builder.Configuration.GetSection(ExpertBridgeRateLimitSettings.SectionName)
    .Bind(rateLimitOptions);

builder.Services.AddRateLimiting(rateLimitOptions);

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();
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
app.UseRateLimiter();
// app.UseRequestLocalization();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<EmailVerifiedMiddleware>();

// app.UseResponseCompression();

// Response caching: 
app.UseResponseCaching();
app.Use(async (context, next) =>
{
    context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
    {
        Private = true, // Sotre on browser cache only. No storing on proxy cahches.
    };

    await next();
});

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
