

using ExpertBridge.Api.Extensions;
using ExpertBridge.Api.Middleware;
using ExpertBridge.Api.Settings;
using ExpertBridge.Api.Settings.Serilog;
using ExpertBridge.Data;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using Microsoft.Net.Http.Headers;
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

builder.Services.Configure<ExpertBridgeRateLimitSettings>(
    builder.Configuration.GetSection(ExpertBridgeRateLimitSettings.SectionName));

builder.Services.AddDatabase(builder.Configuration);
builder.AddSeqEndpoint(connectionName: "Seq");
builder.AddRedisDistributedCache(connectionName: "Redis");
builder.AddS3ObjectService();

builder.AddFirebaseApp();
builder.AddFirebaseAuthentication();
builder.AddHttpClientForFirebaseService();

builder.AddRefitHttpClients();

builder.AddSwaggerGen();
builder.AddCors();

builder.Services.AddHttpContextAccessor();

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

builder.Services.AddSingleton<IEmbeddingGenerator<string, Embedding<float>>>(
    serviceProvider => new OllamaEmbeddingGenerator(
        builder.Configuration["Ollama:Url"]!,
        builder.Configuration["Ollama:ModelId"]!)
);

builder.Services.AddServices();

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
        Private = true, // Store on browser cache only. No storing on proxy caches.
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
