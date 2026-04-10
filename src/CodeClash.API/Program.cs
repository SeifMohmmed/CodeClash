using CodeClash.API;
using CodeClash.API.Extensions;
using CodeClash.API.Settings;
using CodeClash.Application;
using CodeClash.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddApplication(builder.Configuration);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApiServices(builder.Environment);

builder.Services.AddCorsPolicy(builder.Configuration);

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();

    await app.ApplyMigrationsAsync();

    await app.InitializeElasticSearchAsync();

    //app.SeedData();
}

app.UseCustomExceptionHandler();

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors(CorsOptions.PolicyName);

app.UseRequestContextLogging();

app.MapControllers();

await app.RunAsync();
