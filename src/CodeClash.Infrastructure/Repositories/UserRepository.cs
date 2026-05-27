using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Identity;
using Microsoft.EntityFrameworkCore;

namespace CodeClash.Infrastructure.Repositories;
internal sealed class UserRepository(
    ApplicationDbContext context) : IUserRepository
{
    public async Task AddAsync(User user)
    {
        await context.Users.AddAsync(user);
    }

    public async Task<User?> GetByIdentityIdAsync(string identityId)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.IdentityId == identityId);
    }
}
