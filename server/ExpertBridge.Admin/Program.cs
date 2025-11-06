// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Admin.Components;
using ExpertBridge.Admin.Components.Account;
using ExpertBridge.Admin.Database;
using ExpertBridge.Admin.Services;
using ExpertBridge.Data;
using ExpertBridge.Extensions.Caching;
using ExpertBridge.Extensions.CORS;
using ExpertBridge.Extensions.HealthChecks;
using ExpertBridge.Extensions.Logging;
using ExpertBridge.Extensions.MessageBroker;
using ExpertBridge.Extensions.OpenTelemetry;
using ExpertBridge.Notifications.Extensions;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Radzen;

var builder = WebApplication.CreateBuilder(args);
var cacheSection = builder.Configuration.GetSection(CacheSettings.SectionName);

builder.Services
    .AddDatabase(builder.Configuration)
    .Configure<CacheSettings>(cacheSection);

builder
    .RegisterMessageBroker()
    .AddNotifications()
    .AddDefaultHealthChecks()
    .AddCors()
    .AddSerilogLogging()
    .ConfigureOpenTelemetry()
    .ConfigureHttpClientDefaults()
    .AddFusionCache();

builder.Services
    .AddScoped<ModerationReportService>()
    .AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services
    .AddCascadingAuthenticationState()
    .AddScoped<IdentityUserAccessor>()
    .AddScoped<IdentityRedirectManager>()
    .AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>()
    .AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.Services
    .AddAdminDatabase(builder.Configuration)
    .AddDatabaseDeveloperPageExceptionFilter()
    .AddIdentityCore<Admin>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<AdminDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<Admin>, IdentityNoOpEmailSender>();

builder.Services
    .AddRadzenComponents()
    .AddRadzenCookieThemeService()
    .AddRadzenQueryStringThemeService();

var app = builder.Build();

await app.ApplyMigrationAtStartup();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", true);
    app.UseHsts();
}

app.UseCors(CorsPolicyNames.AllowAll);

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapDefaultEndpoints();
app.MapAdditionalIdentityEndpoints();

await app.RunAsync();
