using CodeClash.Application.Behaviors;
using CodeClash.Application.Helpers;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeClash.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        // mediator
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(assembly);

            configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));

            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // fluent validations
        services.AddValidatorsFromAssembly(assembly);

        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

        services.Configure<ElasticSettings>(configuration.GetSection("ElasticSearch"));

        // redis
        services.AddStackExchangeRedisCache(options =>
        options.Configuration = configuration.GetConnectionString("Redis"));

        services.AddMemoryCache();

        return services;
    }

}
