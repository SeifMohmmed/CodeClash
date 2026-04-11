using CodeClash.Domain.Models.Submits;

namespace CodeClash.Domain.Abstractions;
public interface ISubmissionRepository
{
    IQueryable<Submit> GetAllSubmissions(Guid problemId, string userId);

    IQueryable<Submit> GetSolvedSubmissions(Guid problemId, string userId);

    Task<HashSet<Guid>> GetSolvedProblemIdsAsync(List<Guid> problemIds, string userId);
}
