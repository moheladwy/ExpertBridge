using ExpertBridge.Api;
using ExpertBridge.Api.Extensions;
using ExpertBridge.Api.Middlewares;
using ExpertBridge.Application;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.AddDatabase();
builder.AddRedisDistributedCache(connectionName: "Redis");
builder.AddHealthChecks();
builder.Services.AddControllers();
builder.AddLoggingService();
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

if (app.Environment.IsDevelopment())
{
    await app.UseAutoMigrationAtStartup();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health", new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });

await app.RunAsync();
