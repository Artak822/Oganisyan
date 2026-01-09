using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Security.Claims;
using System.Text.Json;
using System.Text;
using System;
using WishList.API.Data;
using WishList.API.Middleware;
using WishList.API.Services;
using WishList.API.Services.Interfaces;
using WishList.API.Repositories;
using WishList.API.Repositories.Interfaces;
using FluentValidation;
using WishList.API.Validators;
using Serilog;
using AspNetCoreRateLimit;
using Prometheus;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Serilog configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WishList API", Version = "v1" });
    
    // JWT Bearer configuration
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "API Key authentication. Example: \"X-API-KEY: {key}\"",
        Name = "X-API-KEY",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
        },
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Database configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)); // Schema is managed by Liquibase, not EF Core migrations

// Redis configuration
var redisConnection = builder.Configuration.GetConnectionString("Redis");
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(redisConnection ?? "localhost:6379"));
builder.Services.AddScoped<ICacheService, CacheService>();

// Dapper configuration
builder.Services.AddScoped<IDapperContext, DapperContext>();

// Repositories
builder.Services.AddScoped<IWishRepository, WishRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IApiKeyRepository, ApiKeyRepository>();

// Services
builder.Services.AddScoped<IWishService, WishService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IApiKeyService, ApiKeyService>();

// Validators
builder.Services.AddValidatorsFromAssemblyContaining<CreateWishDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateWishDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginDtoValidator>();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "DefaultScheme";
    options.DefaultChallengeScheme = "DefaultScheme";
})
    .AddPolicyScheme("DefaultScheme", "JWT or API key", options =>
    {
        options.ForwardDefaultSelector = context =>
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            var hasApiKey = context.Request.Headers.ContainsKey("X-API-KEY");
            
            logger.LogInformation("Auth selector: Authorization header present: {HasAuth}, Value: {AuthValue}, Has API Key: {HasApiKey}", 
                !string.IsNullOrWhiteSpace(authorizationHeader),
                string.IsNullOrWhiteSpace(authorizationHeader) ? "null" : authorizationHeader.Substring(0, Math.Min(50, authorizationHeader.Length)) + "...",
                hasApiKey);
            
            // Check for Bearer token (with or without "Bearer " prefix)
            if (!string.IsNullOrWhiteSpace(authorizationHeader))
            {
                var token = authorizationHeader;
                if (authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    token = authorizationHeader.Substring(7); // Remove "Bearer "
                }
                
                // If it looks like a JWT token (starts with eyJ), use JWT Bearer
                if (token.StartsWith("eyJ", StringComparison.OrdinalIgnoreCase))
                {
                    logger.LogInformation("Selecting JWT Bearer scheme (token detected)");
                    // Set the header with Bearer prefix if it wasn't there
                    if (!authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    {
                        context.Request.Headers["Authorization"] = "Bearer " + token;
                    }
                    return JwtBearerDefaults.AuthenticationScheme;
                }
            }

            if (hasApiKey)
            {
                logger.LogInformation("Selecting API Key scheme");
                return "ApiKey";
            }

            logger.LogWarning("No valid authentication header found, defaulting to JWT Bearer");
            return JwtBearerDefaults.AuthenticationScheme;
        };
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.Name
        };
        
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError(context.Exception, "JWT Authentication failed");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                var role = context.Principal?.FindFirst(ClaimTypes.Role)?.Value;
                var claims = context.Principal?.Claims.Select(c => $"{c.Type}={c.Value}").ToList() ?? new List<string>();
                logger.LogInformation("JWT Token validated. Role: {Role}, Claims: [{Claims}]", 
                    role ?? "null", string.Join(", ", claims));
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogWarning("JWT Challenge triggered. Error: {Error}, ErrorDescription: {ErrorDescription}", 
                    context.Error, context.ErrorDescription);
                return Task.CompletedTask;
            }
        };
    })
    .AddScheme<ApiKeyAuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiKey", options => { });

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("ManagerOrAdmin", policy => policy.RequireRole("Manager", "Admin"));
    options.AddPolicy("UserOrAbove", policy => policy.RequireRole("User", "Manager", "Admin"));
});

// Rate Limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// Health Checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("API is running"))
    .AddNpgSql(connectionString ?? throw new InvalidOperationException("Connection string not found"))
    .AddRedis(redisConnection ?? throw new InvalidOperationException("Redis connection string not found"));

// Prometheus metrics - will be started in app.UseMetricServer()

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Prometheus metrics
app.UseMetricServer();
app.UseHttpMetrics();

// Rate Limiting
app.UseIpRateLimiting();

// Global error handling middleware
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// Request logging middleware
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health check endpoint
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description,
                duration = entry.Value.Duration.TotalMilliseconds
            }),
            totalDuration = report.TotalDuration.TotalMilliseconds
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
});

app.Run();

