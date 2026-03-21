using CodeClash.API.Middleware;
using CodeClash.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CodeClash.API.Extensions;
public static class ApplicationBuilderExtension
{
    /// <summary>
    /// Contains extension method related to database initialization.
    /// </summary>
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        // Create a scoped service provider
        // Required because DbContext is registered as scoped service
        using IServiceScope scope = app.Services.CreateScope();

        // Resolve ApplicationDbContext from DI container
        await using ApplicationDbContext applicationDbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Resolve ApplicationDbContext from DI container
        await using ApplicationIdentityDbContext identityDbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();

        try
        {
            // Apply all pending migrations
            await applicationDbContext.Database.MigrateAsync();
            app.Logger.LogInformation("Application database migrations applied successfully.");

            await identityDbContext.Database.MigrateAsync();
            app.Logger.LogInformation("Identity database migrations applied successfully.");
        }

        catch (Exception ex)
        {
            // Log migration failure
            app.Logger.LogError(ex, "An error occurred while applying database migrations.");

            // Re-throw exception so application fails fast
            throw;
        }

    }

    /// <summary>
    /// Extension method used to register the custom exception handling middleware
    /// in the ASP.NET Core request pipeline.
    /// </summary>
    public static void UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        // Adds ExceptionHandlingMiddleware to the middleware pipeline
        // This middleware catches unhandled exceptions and returns
        // standardized error responses (ProblemDetails).
        app.UseMiddleware<ExceptionHandlingMiddleware>();
    }

    /// <summary>
    /// Extension method used to register the <see cref="RequestContextLoggingMiddleware"/>
    /// in the ASP.NET Core middleware pipeline.
    /// 
    /// This middleware enriches logs with a correlation ID that allows tracking
    /// a single request across multiple services.
    /// </summary>
    public static IApplicationBuilder UseRequestContextLogging(this IApplicationBuilder app)
    {
        // Add the RequestContextLoggingMiddleware to the request pipeline
        app.UseMiddleware<RequestContextLoggingMiddleware>();

        return app;
    }
}
