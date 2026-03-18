using CodeClash.Application.Abstractions.Data;
using CodeClash.Application.Abstractions.Email;
using CodeClash.Application.Abstractions.Execution;
using CodeClash.Application.Abstractions.File;
using CodeClash.Domain.Abstractions;
using CodeClash.Infrastructure.Data;
using CodeClash.Infrastructure.Implementation;
using CodeClash.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeClash.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        AddPersistence(services, configuration);

        services.AddScoped<IProblemRepository, ProblemRepository>();

        services.AddScoped<ITestCaseRepository, TestCaseRepository>();

        services.AddScoped<ISubmissionRepository, SubmissionRepository>();

        services.AddScoped<IContestRepository, ContestRepository>();

        services.AddScoped<ISubmitRepository, SubmitRepository>();

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IFileService, FileService>();

        services.AddScoped<IExecutionService, ExecutionService>();

        services.AddScoped<IEmailService, EmailService>();

        return services;
    }

    /// <summary>
    /// Registers persistence layer services
    /// (EF Core, repositories, Dapper)
    /// </summary>
    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        // Get connection string from configuration
        var connectionString = configuration.GetConnectionString("Database") ??
            throw new ArgumentNullException(nameof(configuration));

        // Register EF Core DbContext with PostgreSQL
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options
                .UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention();
        });

        services.AddSingleton<ISqlConnectionFactory>(_ =>
         new SqlConnectionFactory(connectionString));

    }
}
