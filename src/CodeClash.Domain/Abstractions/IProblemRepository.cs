using CodeClash.Domain.Models.Problems;
using CodeClash.Domain.Models.TestCases;

namespace CodeClash.Domain.Abstractions;
public interface IProblemRepository : IGenericRepository<Problem>
{
    IQueryable<Testcase> GetTestCasesByProblemId(
        Guid problemId);

    Task<Problem?> GetProblemDetailsAsync(
        Guid problemId,
        CancellationToken cancellationToken = default);

    Task<int> GetAcceptedProblemCountAsync(
        Guid problemId,
        CancellationToken cancellationToken = default);
    Task<int> GetSubmissionsProblemCountAsync(
        Guid problemId,
        CancellationToken cancellationToken = default);

    Task<bool> CheckUserSolvedProblemAsync(
        Guid problemId,
        string userId,
        CancellationToken cancellationToken = default);
}
