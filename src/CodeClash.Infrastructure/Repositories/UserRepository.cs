using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Identity;

namespace CodeClash.Infrastructure.Repositories;
internal sealed class UserRepository(
    ApplicationDbContext context) : IUserRepository
{
    public async Task AddAsync(User user)
    {
        await context.Users.AddAsync(user);
    }
}
