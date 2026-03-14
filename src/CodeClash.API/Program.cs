using CodeClash.API;
using CodeClash.API.Extensions;
using CodeClash.Application;
using CodeClash.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddApiServices()
    .AddObservability();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
}

app.UseHttpsRedirection();

app.MapControllers();

await app.RunAsync();
