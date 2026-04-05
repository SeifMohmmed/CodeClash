using System.Text;
using CodeClash.Application.Abstractions.Data;
using CodeClash.Application.Abstractions.Email;
using CodeClash.Application.Abstractions.Execution;
using CodeClash.Application.Abstractions.File;
using CodeClash.Application.Abstractions.Identity;
using CodeClash.Application.Abstractions.Roles;
using CodeClash.Application.Helpers;
using CodeClash.Domain.Abstractions;
using CodeClash.Infrastructure.Data;
using CodeClash.Infrastructure.Implementation;
using CodeClash.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace CodeClash.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        AddPersistence(services, configuration);

        AddAuthentication(services, configuration);

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

        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IRoleService, RoleService>();

        services.AddScoped<ITokenProvider, TokenProvider>();

        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

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

        services.AddScoped<IIdentityDbContext, ApplicationIdentityDbContext>();

        services.AddScoped<IAppDbContext, ApplicationDbContext>();

    }

    /// <summary>
    /// Configures ASP.NET Core Identity + JWT authentication.
    /// </summary>
    private static void AddAuthentication(
    IServiceCollection services,
    IConfiguration configuration)
    {
        services
                // Registers ASP.NET Core Identity with default user & role entities
                .AddIdentity<IdentityUser, IdentityRole>()
                // Configures EF Core store for Identity
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                .AddDefaultTokenProviders();

        // Bind Jwt settings from configuration (appsettings.json → Jwt section)
        services.Configure<JwtAuthOptions>(configuration.GetSection("Jwt"));

        // Retrieve strongly typed Jwt settings
        JwtAuthOptions jwtAuthOptions = configuration.GetSection("Jwt").Get<JwtAuthOptions>()!;

        services
            // Configures authentication services and sets default schemes
            // DefaultAuthenticateScheme → used when validating incoming requests
            // DefaultChallengeScheme → used when returning 401 Unauthorized responses
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // Registers JWT Bearer authentication handler
            // This enables the API to accept tokens via:
            // Authorization: Bearer {token}
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Validates the token issuer to ensure it was generated by trusted authority
                    ValidIssuer = jwtAuthOptions.Issuer,

                    // Validates the intended audience of the token
                    ValidAudience = jwtAuthOptions.Audience,

                    // Validates token signature using symmetric security key
                    // Prevents tampering with token payload
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtAuthOptions.Key))
                };
            });

        // Enables authorization policies
        services.AddAuthorization();

    }
}
