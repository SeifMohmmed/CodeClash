using CodeClash.Domain.Models.Submits;
using CodeClash.Domain.Premitives;

namespace CodeClash.Domain.Abstractions;

public interface ISubmissionRepository
{
    Task<Submit?> GetByIdAsync(Guid id);

    Task<IReadOnlyList<Submit>> GetAllSubmissions(Guid problemId, string userId);

    Task<IReadOnlyList<Submit>> GetSolvedSubmissions(Guid problemId, string userId);

    Task<HashSet<Guid>> GetUserAcceptedSubmissions(string userId);

    Task<HashSet<Guid>> GetSolvedProblemIdsAsync(List<Guid> problemIds, string userId);

    Task<Dictionary<Guid, SubmissionResult>> GetUserSubmissionsAsync(string userId);

    Task<Submit?> GetSubmissionIfAuthorized(string userId, Guid submissionId);

    Task<List<Submit>> GetContestACSubmissionsByProblemIdsAsync(Guid contestId, List<Guid> problemIds);
}
