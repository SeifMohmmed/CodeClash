using CodeClash.Domain.Models.Contests;
using CodeClash.Domain.Models.Problems;

namespace CodeClash.Domain.Abstractions;
public interface IContestRepository : IGenericRepository<Contest>
{
    Task<IReadOnlyList<Problem>> GetContestProblemsByIdAsync(Guid contestId);
    Task<int> GetProblemCountAsync(Guid contestId);
    Task<bool> HasProblemAsync(Guid contestId, Guid problemId);
    Task AddProblemAsync(Guid contestId, Guid problemId);
}
