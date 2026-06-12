using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Contests;
using CodeClash.Domain.Models.Problems;
using CodeClash.Domain.Premitives;
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
            .Include(x => x.Registrations)
            .Include(x => x.Problems)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<IReadOnlyList<(Contest, bool)>> GetAllContestWithRegisteredUserAsync(
        string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<StandingDto>> GetContestStanding(
      Guid contestId,
      int index,
      int pageSize)
    {
        var contest = await _context.Contests
            .Where(c => c.Id == contestId)
            .Select(c => new { c.StartDate, c.EndDate })
            .FirstOrDefaultAsync();

        if (contest is null)
        {
            return [];
        }

        // First accepted submission per (user, problem), within contest window
        var firstAccepted = _context.Submits
            .Where(s => s.ContestId == contestId
                     && s.Result == SubmissionResult.Accepted
                     && s.SubmissionDate >= contest.StartDate
                     && s.SubmissionDate <= contest.EndDate)
            .GroupBy(s => new { s.UserId, s.ProblemId })
            .Select(g => new
            {
                g.Key.UserId,
                g.Key.ProblemId,
                FirstSubmission = g.Min(s => s.SubmissionDate)
            });

        var standing = await firstAccepted
            .Join(_context.Problems,
                  s => s.ProblemId,
                  p => p.Id,
                  (s, p) => new { s.UserId, s.FirstSubmission, Points = (int)p.ContestPoints })
            .GroupBy(s => s.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                TotalPoints = g.Sum(s => s.Points),
                LastSubmission = g.Max(s => s.FirstSubmission) // tiebreaker
            })
            .OrderByDescending(s => s.TotalPoints)
            .ThenBy(s => s.LastSubmission)
            .Skip(index * pageSize)
            .Take(pageSize)
            .Join(_context.Users,
                  s => s.UserId,
                  u => u.Id,
                  (s, u) => new StandingDto
                  {
                      UserId = s.UserId,
                      UserName = u.Name,
                      UserImage = u.ImagePath,
                  })
            .ToListAsync();

        // Assign rank based on page position
        for (int i = 0; i < standing.Count; i++)
        {
            standing[i].Rank = index * pageSize + i + 1;
        }

        return standing;
    }
}
