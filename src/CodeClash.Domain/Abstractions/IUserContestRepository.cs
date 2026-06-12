using CodeClash.Domain.Models.Contests;

namespace CodeClash.Domain.Abstractions;

public interface IUserContestRepository
{
    Task<UserContest?> GetUserContest(
        string userId,
        Guid contestId);

    Task AddAsync(
        UserContest registration,
        CancellationToken cancellationToken = default);

    Task<bool> IsRegistered(
        Guid contestId,
        string userId,
        CancellationToken cancellationToken = default);
}
