using Amazon.Runtime;
using Amazon.S3;
using ExpertBridge.Application;
using ExpertBridge.Application.Services;
using ExpertBridge.Core.Interfaces;
using ExpertBridge.Core.Interfaces.Services;
using ExpertBridge.Data.DatabaseContexts;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace ExpertBridge.Api;

internal static class Startup
{
    /// <summary>
    ///     Adds the database service to the application builder.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder to add the database service to.</param>
    public static void AddDatabase(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("Postgresql")!;
        builder.Services.AddDbContext<ExpertBridgeDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });
    }

    /// <summary>
    ///     Add Health Checks to the application builder, with database health check.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder to add the health checks to.</param>
    public static void AddHealthChecks(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("Postgresql")!;
        builder.Services.AddHealthChecks().AddNpgSql(connectionString: connectionString);
    }

    /// <summary>
    ///     Adds the Firebase app, auth, and messaging services to the application builder.
    /// </summary>
    /// <param name="builder">
    ///     The WebApplicationBuilder to add the Firebase services to.
    /// </param>
    public static void AddFirebaseServices(this WebApplicationBuilder builder)
    {
        var firebaseApp = FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromFile("FirebaseOAuthCredentialsExpertBridge.json")
        });
        var firebaseAuth = FirebaseAuth.GetAuth(firebaseApp);
        var firebaseMessaging = FirebaseMessaging.GetMessaging(firebaseApp);

        builder.Services.AddSingleton(firebaseApp);
        builder.Services.AddSingleton(firebaseAuth);
        builder.Services.AddSingleton(firebaseMessaging);
    }

    /// <summary>
    ///     Adds the Firebase authentication service to the application builder.
    /// </summary>
    /// <param name="builder">builder — The WebApplicationBuilder to add the Firebase authentication services to</param>
    public static void AddAuthentication(this WebApplicationBuilder builder)
    {
        var projectId = builder.Configuration["Firebase:ProjectId"]!;
        var issuer = builder.Configuration["Authentication:Firebase:Issuer"]!;
        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.IncludeErrorDetails = true;
                options.Authority = issuer;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = projectId,
                    ValidateLifetime = true
                };
                options.RequireHttpsMetadata = false;
            });
        builder.Services.AddAuthorization();
    }

    /// <summary>
    ///     Adds the Swagger UI service to the application builder.
    /// </summary>
    /// <param name="builder">
    ///     The WebApplicationBuilder to add the Swagger UI service to.
    /// </param>
    public static void AddSwaggerGen(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "ExpertBridge API", Version = "v1" });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter your JWT token in this field: {your token}",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }

    /// <summary>
    ///     Adds the HttpClient service for the Firebase service.
    /// </summary>
    /// <param name="builder">
    ///     The WebApplicationBuilder to add the HttpClient service to.
    /// </param>
    public static void AddHttpClientForFirebaseService(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpClient<IFirebaseService, FirebaseService>((sp, httpClient) =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var uri = configuration["Authentication:Firebase:TokenUri"]!;
            httpClient.BaseAddress = new Uri(uri);
        });
    }

    /// <summary>
    ///     Adds the S3 object service to the application builder.
    /// </summary>
    /// <param name="builder">
    ///     The WebApplicationBuilder to add the S3 object service to.
    /// </param>
    public static void AddS3ObjectService(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<AwsConfigurations>(builder.Configuration.GetSection("AwsS3"));
        builder.Services.AddSingleton<IAmazonS3>(sp =>
        {
            var awsConfig = sp.GetRequiredService<IOptions<AwsConfigurations>>().Value;
            var credentials = new BasicAWSCredentials(awsConfig.Awskey, awsConfig.AwsSecret);
            var configurations = new AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(awsConfig.Region)
            };
            return new AmazonS3Client(credentials, configurations);
        });
    }

    /// <summary>
    ///     Adds the logging service to the application builder.
    /// </summary>
    /// <param name="builder">
    ///     The WebApplicationBuilder to add the logging service to.
    /// </param>
    public static void AddLoggingService(this WebApplicationBuilder builder)
    {
        builder.Services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddConsole()
                .AddFilter("Microsoft.AspNetCore.Authentication", LogLevel.Debug)
                .AddFilter("Microsoft.AspNetCore.Authorization", LogLevel.Debug);
        });
        builder.Services.AddHttpLogging(configureOptions: options =>
        {
            options.LoggingFields = HttpLoggingFields.All;
            options.RequestBodyLogLimit = 4096;
            options.ResponseBodyLogLimit = 4096;
            options.RequestHeaders.Add("Authorization");
            options.ResponseHeaders.Add("Authorization");
        });
    }
}
