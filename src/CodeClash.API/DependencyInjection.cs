using CodeClash.API.Settings;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace CodeClash.API;

/// <summary>
/// Centralized extension methods for registering application services.
/// This keeps Program.cs clean and organized.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IHostEnvironment environment)
    {
        services.AddControllers();

        services.AddSwaggerGen();

        AddObservability(services, environment);

        return services;
    }

    public static IServiceCollection AddObservability(
        this IServiceCollection services,
        IHostEnvironment environment)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(resource =>
                // Register service name in telemetry system
                resource.AddService(environment.ApplicationName))
            .WithTracing(tracing => tracing
                // Trace outgoing HTTP calls
                .AddHttpClientInstrumentation()
                // Trace incoming ASP.NET Core requests
                .AddAspNetCoreInstrumentation()
                // Trace PostgreSQL queries
                .AddNpgsql())
            .WithMetrics(metrics => metrics
                // Metrics for outgoing HTTP
                .AddHttpClientInstrumentation()
                // Metrics for incoming HTTP
                .AddAspNetCoreInstrumentation()
                // Runtime metrics (GC, CPU, etc.)
                .AddRuntimeInstrumentation())
            // Export telemetry using OTLP (e.g., to Jaeger, Grafana, etc.)
            .UseOtlpExporter();

        // Adds OpenTelemetry logging to capture structured logs
        services.AddLogging(logging =>
        {
            logging.AddOpenTelemetry(options =>
            {
                options.IncludeScopes = true;
                options.IncludeFormattedMessage = true;
            });
        });

        return services;
    }

    public static IServiceCollection AddCorsPolicy(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        // Load CORS settings from configuration
        var corsOptions = configuration
            .GetSection(CorsOptions.SectionName)
            .Get<CorsOptions>()!;

        services.AddCors(options =>
        {
            options.AddPolicy(CorsOptions.PolicyName, policy =>
            {
                // Allow configured origins
                policy
                    .WithOrigins(corsOptions.AllowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        return services;
    }
}
