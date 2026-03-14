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
    public static WebApplicationBuilder AddApiServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen();

        return builder;
    }

    public static WebApplicationBuilder AddObservability(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource =>
                // Register service name in telemetry system
                resource.AddService(builder.Environment.ApplicationName))
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
        builder.Logging.AddOpenTelemetry(options =>
        {
            // Includes logging scopes (useful for request tracing and correlation IDs)
            options.IncludeScopes = true;

            // Includes the fully formatted log message instead of only template + parameters
            options.IncludeFormattedMessage = true;
        });

        return builder;
    }
}
