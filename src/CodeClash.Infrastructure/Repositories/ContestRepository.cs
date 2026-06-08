using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Contests;
using CodeClash.Domain.Models.Problems;
using Microsoft.EntityFrameworkCore;

namespace CodeClash.Infrastructure.Repositories;

internal sealed class ContestRepository : GenericRepository<Contest>, IContestRepository
{
    private readonly ApplicationDbContext _context;
    public ContestRepository(ApplicationDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<int> GetProblemCountAsync(Guid contestId) =>
        await _context.Problems.CountAsync(p => p.ContestId == contestId);

    public async Task<bool> HasProblemAsync(Guid contestId, Guid problemId) =>
    await _context.Problems.AnyAsync(p => p.ContestId == contestId && p.Id == problemId);

    public async Task<IReadOnlyList<Problem>> GetContestProblemsByIdAsync(
        Guid contestId)
    {
        var problems = await _context.Problems.Where(x => x.ContestId == contestId)
                            .ToListAsync();

        return problems;
    }

    public async Task AddProblemAsync(Guid contestId, Guid problemId)
    {
        var problem = await _context.Problems.FindAsync(problemId);
        if (problem is not null)
        {
            problem.ContestId = contestId;
        }
    }

    public async Task<IReadOnlyList<Contest>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<Contest>()
            .AsNoTracking()
            .Include(x => x.Registrations)
            .Include(x => x.Problems)
            .ToListAsync(cancellationToken);
    }

    public Task<IReadOnlyList<(Contest, bool)>> GetAllContestWithRegisteredUserAsync(
        string userId)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<StandingDto>> GetContestStanding(Guid contestId)
    {
        throw new NotImplementedException();
    }
}
