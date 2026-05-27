using CodeClash.Domain.Models.Contests;

namespace CodeClash.Domain.Abstractions;
public interface IUserContestRepository
{
    Task<UserContest?> GetUserContest(
        string userId,
        Guid contestId);

    Task<bool> RegisterInContest(UserContest registration);

    Task<bool> IsRegistered(
        Guid contestId,
        string userId);
}
