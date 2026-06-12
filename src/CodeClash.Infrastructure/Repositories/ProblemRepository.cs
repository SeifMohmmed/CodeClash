using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Problems;
using CodeClash.Domain.Models.Submits;
using CodeClash.Domain.Models.TestCases;
using CodeClash.Domain.Premitives;
using Microsoft.EntityFrameworkCore;

namespace CodeClash.Infrastructure.Repositories;

internal sealed class ProblemRepository : GenericRepository<Problem>, IProblemRepository
{
    private readonly ApplicationDbContext _context;
    public ProblemRepository(ApplicationDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<bool> CheckUserSolvedProblemAsync(
        Guid problemId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<Submit>()
            .AnyAsync(p => p.ProblemId == problemId
                        && p.UserId == userId
                        && p.Result == SubmissionResult.Accepted,
                    cancellationToken);
    }

    public async Task<Problem?> GetProblemDetailsAsync(
        Guid problemId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<Problem>()
            .Include(x => x.Testcases)
            .Include(y => y.ProblemTopics)
                .ThenInclude(x => x.Topic)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == problemId, cancellationToken);
    }

    public async Task<Problem?> GetProblemWithContestAndTestcasesAsync(
        Guid problemId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<Problem>()
            .Include(x => x.Contest)
            .Include(y => y.Testcases)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == problemId, cancellationToken);
    }

    public async Task<ProblemStats> GetProblemStatsAsync(
        Guid problemId,
        CancellationToken cancellationToken = default)
    {
        // Single round-trip: group all submissions for the problem and
        // count accepted vs total in the database, not in memory.
        var stats = await _context.Set<Submit>()
            .Where(s => s.ProblemId == problemId)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                AcceptedCount = g.Count(s => s.Result == SubmissionResult.Accepted),
                TotalCount = g.Count()
            })
            .FirstOrDefaultAsync(cancellationToken);

        // No submissions yet — return zeroed stats rather than null.
        return stats is null
            ? new ProblemStats(0, 0)
            : new ProblemStats(stats.AcceptedCount, stats.TotalCount);
    }

    public async Task<IReadOnlyList<Testcase>> GetTestCasesByProblemIdAsync(
        Guid problemId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<Testcase>()
                .Where(tc => tc.ProblemId == problemId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
    }
}
