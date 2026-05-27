using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Contests;
using Microsoft.EntityFrameworkCore;

namespace CodeClash.Infrastructure.Repositories;
internal sealed class UserContestRepository(
    ApplicationDbContext context) : IUserContestRepository
{
    public async Task<UserContest?> GetUserContest(
        string userId,
        Guid contestId)
    {
        return await context.Registers.FirstOrDefaultAsync(
            r => r.UserId == userId
            && r.ContestId == contestId);
    }

    public async Task<bool> IsRegistered(
        Guid contestId,
        string userId)
    {
        return await context.Registers.AnyAsync(
            r => r.UserId == userId
            && r.ContestId == contestId);
    }

    public async Task<bool> RegisterInContest(UserContest registration)
    {
        await context.Registers.AddAsync(registration);
        await context.SaveChangesAsync();
        return true;
    }
}
