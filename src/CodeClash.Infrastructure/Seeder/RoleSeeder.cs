using CodeClash.Domain.Premitives;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CodeClash.Infrastructure.Seeder;
public static class RoleSeeder
{
    public static async Task SeedAsync(RoleManager<IdentityRole> _roleManager)
    {
        if (!await _roleManager.Roles.AnyAsync())
        {
            await _roleManager.CreateAsync(new IdentityRole(Roles.Admin));

            await _roleManager.CreateAsync(new IdentityRole(Roles.User));
        }
    }
}
