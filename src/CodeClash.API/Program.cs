using CodeClash.API;
using CodeClash.API.Extensions;
using CodeClash.Application;
using CodeClash.Domain.Models.Identity;
using CodeClash.Infrastructure;
using CodeClash.Infrastructure.Context;
using CodeClash.Infrastructure.Seeder;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddApiServices()
    .AddObservability();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();


builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
}

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await RoleSeeder.SeedAsync(roleManager);
    await UserSeeder.SeedAsync(userManager);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
