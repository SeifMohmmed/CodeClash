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

    public async Task<int> GetAcceptedProblemCountAsync(
        Guid problemId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<Submit>()
            .CountAsync(p => p.ProblemId == problemId
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
            .FirstOrDefaultAsync(x => x.Id == problemId, cancellationToken);
    }

    public async Task<int> GetSubmissionsProblemCountAsync(
        Guid problemId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<Submit>()
            .CountAsync(p => p.ProblemId == problemId, cancellationToken);
    }

    public IQueryable<Testcase> GetTestCasesByProblemId(Guid problemId)
    {
        return _context.Set<Testcase>()
                .Where(x => x.ProblemId == problemId);
    }
}
