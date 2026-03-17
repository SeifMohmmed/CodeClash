using CodeClash.API.Middleware;
using CodeClash.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CodeClash.API.Extensions;

public static class ApplicationBuilderExtension
{
    /// <summary>
    /// Run EF Migrations only for internal development - DEV
    /// </summary>
    /// <param name="app"></param>
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Database.Migrate();
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
}
