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

    public async Task<IReadOnlyList<Problem>> GetContestProblemsByIdAsync(
        Guid contestId)
    {
        var problems = await _context.Problems.Where(x => x.ContestId == contestId)
                            .ToListAsync();

        return problems;
    }
}
