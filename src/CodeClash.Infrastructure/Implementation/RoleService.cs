using CodeClash.Application.Abstractions.Roles;
using CodeClash.Domain.Models.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CodeClash.Infrastructure.Implementation;
internal sealed class RoleService(
    ApplicationIdentityDbContext context,
    RoleManager<IdentityRole> roleManager,
    UserManager<IdentityUser> userManager) : IRoleService
{
    public async Task<Role> CreateRoleAsync(string roleName)
    {
        //  Check existence first
        if (await roleManager.RoleExistsAsync(roleName))
        {
            throw new InvalidOperationException($"Role '{roleName}' already exists.");
        }

        var identityRole = new IdentityRole(roleName);
        var result = await roleManager.CreateAsync(identityRole);

        // Handle Identity errors
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to create role: {errors}");
        }

        // Map to domain model
        return new Role { Id = identityRole.Id, Name = identityRole.Name! };
    }

    public async Task<IEnumerable<Role>> GetAllRolesAsync()
    {
        return await context.Roles
            .Select(r => new Role { Id = r.Id, Name = r.Name! })
            .ToListAsync();
    }

    public async Task<Role> GetRoleByNameAsync(string name)
    {
        var identityRole = await roleManager.FindByNameAsync(name)
            ?? throw new KeyNotFoundException($"Role '{name}' was not found.");

        return new Role { Id = identityRole.Id, Name = identityRole.Name! };
    }

    public async Task AssignRoleAsync(string userId, string roleName)
    {
        // Validate user exists
        var user = await userManager.FindByIdAsync(userId)
            ?? throw new KeyNotFoundException($"User '{userId}' was not found.");

        // Validate role exists
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            throw new KeyNotFoundException($"Role '{roleName}' was not found.");
        }

        // Check if user already has the role
        if (await userManager.IsInRoleAsync(user, roleName))
        {
            throw new InvalidOperationException($"User already has role '{roleName}'.");
        }

        var result = await userManager.AddToRoleAsync(user, roleName);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to assign role: {errors}");
        }
    }
}
