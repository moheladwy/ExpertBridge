// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Admin.Components;
using ExpertBridge.Data;
using ExpertBridge.Extensions.Caching;
using ExpertBridge.Extensions.CORS;
using ExpertBridge.Extensions.HealthChecks;
using ExpertBridge.Extensions.Logging;
using ExpertBridge.Extensions.OpenTelemetry;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabase(builder.Configuration);

builder.Services.Configure<CacheSettings>(
    builder.Configuration.GetSection(CacheSettings.SectionName));
builder.AddDefaultHealthChecks();
builder.AddCors();
builder.AddSerilogLogging();
builder.ConfigureOpenTelemetry();
builder.ConfigureHttpClientDefaults();
builder.AddFusionCache();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services
    .AddRadzenComponents()
    .AddRadzenCookieThemeService()
    .AddRadzenQueryStringThemeService();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCors(CorsPolicyNames.AllowAll);

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapDefaultEndpoints();

app.Run();
