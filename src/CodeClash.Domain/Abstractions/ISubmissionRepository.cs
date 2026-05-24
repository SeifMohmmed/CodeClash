using CodeClash.Domain.Models.Submits;
using CodeClash.Domain.Premitives;

namespace CodeClash.Domain.Abstractions;
public interface ISubmissionRepository
{
    Task<Submit?> GetByIdAsync(Guid id);

    IQueryable<Submit> GetAllSubmissions(Guid problemId, string userId);

    IQueryable<Submit> GetSolvedSubmissions(Guid problemId, string userId);

    Task<HashSet<Guid>> GetUserAcceptedSubmissions(string userId);

    Task<HashSet<Guid>> GetSolvedProblemIdsAsync(List<Guid> problemIds, string userId);

    Task<Dictionary<Guid, SubmissionResult>> GetUserSubmissionsAsync(string userId);
}
