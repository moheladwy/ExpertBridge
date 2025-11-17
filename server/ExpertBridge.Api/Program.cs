using System;
using System.Linq;
using System.Text.Json.Serialization;
using ExpertBridge.Api.Extensions;
using ExpertBridge.Api.Filters;
using ExpertBridge.Api.Middleware;
using ExpertBridge.Application.Settings;
using ExpertBridge.Extensions.CORS;
using ExpertBridge.Extensions.HealthChecks;
using ExpertBridge.Extensions.MessageBroker;
using ExpertBridge.Notifications;
using ExpertBridge.Notifications.Extensions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
builder.AddNotifications();

builder.AddSwaggerGen("ExpertBridgeApi");
builder.AddCors();

builder.Services.AddHttpContextAccessor();

builder.Services.AddResponseCaching();

// Configure automatic model validation error response format
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
            );

        var result = new
        {
            Title = "Validation Failed",
            Status = 400,
            Errors = errors,
            TraceId = context.HttpContext.TraceIdentifier
        };

        return new BadRequestObjectResult(result) { StatusCode = StatusCodes.Status400BadRequest };
    };
});

builder.Services.AddControllers(options =>
    {
        // Add global validation filter
        options.Filters.Add<ValidationFilter>();

        options.CacheProfiles.Add(CacheProfiles.Default,
            new CacheProfile { Duration = 300, Location = ResponseCacheLocation.Any });

        options.CacheProfiles.Add(CacheProfiles.PersonalizedContent,
            new CacheProfile { Duration = 180, Location = ResponseCacheLocation.Any, VaryByHeader = "Authorization" });
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        // You can also set options.JsonSerializerOptions.PropertyNameCaseInsensitive = true; if needed for property names,
        // but JsonStringEnumConverter usually handles enum value casing well.
    });

builder.RegisterMessageBroker(typeof(Program).Assembly);

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
        Private = true // Store on browser cache only. No storing on proxy caches.
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
    public class Program
    {
    }
}
