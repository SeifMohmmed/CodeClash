using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Submits;
using CodeClash.Domain.Premitives;
using Microsoft.EntityFrameworkCore;

namespace CodeClash.Infrastructure.Repositories;
internal sealed class SubmissionRepository : GenericRepository<Submit>, ISubmissionRepository
{
    private readonly ApplicationDbContext _context;

    public SubmissionRepository(
        ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public IQueryable<Submit> GetAllSubmissions(Guid problemId, string userId)
     => _context.Submits.Where(x => x.ProblemId == problemId && x.UserId == userId);

    public IQueryable<Submit> GetSolvedSubmissions(Guid problemId, string userId)
      => _context.Submits.Where(x => x.ProblemId == problemId && x.UserId == userId && x.Result == SubmissionResult.Accepted);

    public async Task<HashSet<Guid>> GetSolvedProblemIdsAsync(
    List<Guid> problemIds,
    string userId)
    {
        var solvedIds = await _context.Submits
            .Where(s => problemIds.Contains(s.ProblemId)
                     && s.UserId == userId
                     && s.Result == SubmissionResult.Accepted)
            .Select(s => s.ProblemId)
            .ToListAsync();

        return solvedIds.ToHashSet();
    }
}
