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
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await context.Registers.AnyAsync(
            r => r.UserId == userId
            && r.ContestId == contestId,
            cancellationToken);
    }

    public async Task AddAsync(
        UserContest registration,
        CancellationToken cancellationToken = default)
    {
        await context.Registers.AddAsync(registration, cancellationToken);
    }
}
