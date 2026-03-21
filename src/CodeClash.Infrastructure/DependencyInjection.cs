using CodeClash.Application.Abstractions.Data;
using CodeClash.Application.Abstractions.Email;
using CodeClash.Application.Abstractions.Execution;
using CodeClash.Application.Abstractions.File;
using CodeClash.Application.Abstractions.Identity;
using CodeClash.Domain.Abstractions;
using CodeClash.Infrastructure.Data;
using CodeClash.Infrastructure.Implementation;
using CodeClash.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
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

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IFileService, FileService>();

        services.AddScoped<IExecutionService, ExecutionService>();

        services.AddScoped<IEmailService, EmailService>();

        services.AddScoped<IIdentityService, IdentityService>();

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
                .UseNpgsql(
                connectionString,
                npgsqlOptions =>
                npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName,
                Schemas.Application))
                .UseSnakeCaseNamingConvention();
        });

        services.AddDbContext<ApplicationIdentityDbContext>(options =>
        options.UseNpgsql(
        connectionString,
        npgsqlOptions =>
            // Store EF migrations history in custom schema
            npgsqlOptions.MigrationsHistoryTable(
                HistoryRepository.DefaultTableName,
                Schemas.Identity))
        // Use snake_case naming convention for DB tables/columns
        .UseSnakeCaseNamingConvention());

        services.AddSingleton<ISqlConnectionFactory>(_ =>
         new SqlConnectionFactory(connectionString));

        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationIdentityDbContext>();

    }
}
