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

    public async Task<IReadOnlyList<StandingDto>> GetContestStanding(Guid contestId)
    {
        // Step 1: fetch all accepted submissions with needed fields — let EF translate a flat query
        var acceptedSubmissions = await _context.Submits
            .Where(s => s.ContestId == contestId && s.Result == SubmissionResult.Accepted)
            .Select(s => new
            {
                s.UserId,
                s.ProblemId,
                s.SubmissionDate,
                s.User.Name,
                s.User.ImagePath,
                ContestPoints = (int)s.Problem.ContestPoints
            })
            .ToListAsync();

        // Step 2: aggregate in memory
        var standing = acceptedSubmissions
            .GroupBy(s => new { s.UserId, s.ProblemId })
            .Select(g => g.OrderBy(s => s.SubmissionDate).First()) // first accepted per problem
            .GroupBy(s => s.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                UserName = g.First().Name,
                UserImage = g.First().ImagePath,
                Points = g.Sum(s => s.ContestPoints)
            })
            .OrderByDescending(s => s.Points)
            .Select((s, index) => new StandingDto
            {
                UserId = s.UserId,
                UserName = s.UserName,
                UserImage = s.UserImage,
                Rank = index + 1
            })
            .ToList();

        return standing;
    }
}
