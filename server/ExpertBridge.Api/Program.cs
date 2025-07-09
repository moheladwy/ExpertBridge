using ExpertBridge.Api.Extensions;
using ExpertBridge.Api.Middleware;
using ExpertBridge.Application.Settings;
using ExpertBridge.Extensions.CORS;
using ExpertBridge.Extensions.HealthChecks;
using ExpertBridge.Notifications;
using ExpertBridge.Notifications.Extensions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.AddDefaultHealthChecks();
builder.Services.AddServiceDiscovery();
builder.Services.ConfigureHttpClientDefaults(http =>
{
    http.AddStandardResilienceHandler(options =>
    {
        options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(120);
        options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(60);
        options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(120);
    });
    http.AddServiceDiscovery();
});

builder.AddExpertBridgeServices();
builder.Services.AddExpertBridgeNotifications();

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
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    // You can also set options.JsonSerializerOptions.PropertyNameCaseInsensitive = true; if needed for property names,
    // but JsonStringEnumConverter usually handles enum value casing well.
});

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
app.UseCors(CorsPolicyNames.SignalRClients);

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
app.MapHub<NotificationsHub>("/api/notificationsHub");
//app.MapHub<ChatHub>("/chatHub");
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
