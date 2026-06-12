using CodeClash.Domain.Models.Problems;
using CodeClash.Domain.Models.TestCases;
using CodeClash.Domain.Premitives;

namespace CodeClash.Domain.Abstractions;

public interface IProblemRepository : IGenericRepository<Problem>
{
    Task<IReadOnlyList<Testcase>> GetTestCasesByProblemIdAsync(
        Guid problemId,
        CancellationToken cancellationToken = default);

    Task<Problem?> GetProblemDetailsAsync(
        Guid problemId,
        CancellationToken cancellationToken = default);

    Task<ProblemStats> GetProblemStatsAsync(
    Guid problemId,
    CancellationToken cancellationToken = default);

    Task<Problem?> GetProblemWithContestAndTestcasesAsync(
        Guid problemId,
        CancellationToken cancellationToken = default);

    Task<bool> CheckUserSolvedProblemAsync(
        Guid problemId,
        string userId,
        CancellationToken cancellationToken = default);
}
