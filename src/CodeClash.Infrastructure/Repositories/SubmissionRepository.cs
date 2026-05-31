using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Submits;
using CodeClash.Domain.Premitives;
using Microsoft.EntityFrameworkCore;

namespace CodeClash.Infrastructure.Repositories;

internal sealed class SubmissionRepository : ISubmissionRepository
{
    private readonly ApplicationDbContext _context;

    public SubmissionRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Submit?> GetByIdAsync(Guid id)
        => await _context.Submits.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<IReadOnlyList<Submit>> GetAllSubmissions(
        Guid problemId,
        string userId)
    {
        return await _context.Submits
        .Where(x => x.ProblemId == problemId && x.UserId == userId)
        .ToListAsync();
    }

    public async Task<IReadOnlyList<Submit>> GetSolvedSubmissions(
        Guid problemId,
        string userId)
    {
        return await _context.Submits
        .Where(x => x.ProblemId == problemId && x.UserId == userId && x.Result == SubmissionResult.Accepted)
        .ToListAsync();
    }

    public async Task<HashSet<Guid>> GetUserAcceptedSubmissions(
        string userId)
    {
        var problemsIds = await _context.Submits
            .Where(s => s.UserId == userId
            && s.Result == SubmissionResult.Accepted)
            .Select(s => s.ProblemId).ToListAsync();

        return new HashSet<Guid>(problemsIds);
    }

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

    public async Task<Dictionary<Guid, SubmissionResult>> GetUserSubmissionsAsync(
        string userId)
    {
        return await _context.Submits
            .Where(s => s.UserId == userId)
            .GroupBy(s => s.ProblemId)
            .Select(g => new
            {
                ProblemId = g.Key,
                Result = g.Any(s => s.Result == SubmissionResult.Accepted)
                    ? SubmissionResult.Accepted
                    : g.OrderByDescending(s => s.SubmissionDate).First().Result
            })
            .ToDictionaryAsync(x => x.ProblemId, x => x.Result);
    }

    public async Task<Submit?> GetSubmissionIfAuthorized(
        string userId,
        Guid submissionId)
    {
        var submission = await _context.Submits
            .Include(x => x.Contest)
            .FirstOrDefaultAsync(x => x.Id == submissionId);

        if (submission is null)
        {
            return null;
        }

        if (submission.Contest is not null
            && submission.Contest.ContestStatus == ContestStatus.Running
            && submission.UserId != userId)
        {
            return null;
        }

        return submission;
    }

    public async Task<List<Submit>> GetContestACSubmissionsByProblemIdsAsync(
        Guid contestId,
        List<Guid> problemIds)
    {
        return await _context.Submits
            .Where(s => s.ContestId == contestId &&
                        problemIds.Contains(s.ProblemId) &&
                        s.Result == SubmissionResult.Accepted)
            .ToListAsync();
    }
}
