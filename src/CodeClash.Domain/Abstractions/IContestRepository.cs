using CodeClash.Domain.Models.Contests;
using CodeClash.Domain.Models.Problems;

namespace CodeClash.Domain.Abstractions;
public interface IContestRepository : IGenericRepository<Contest>
{
    Task<IReadOnlyList<Problem>> GetContestProblemsByIdAsync(Guid contestId);
}
