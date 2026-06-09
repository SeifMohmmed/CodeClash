using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Abstractions.RankUp;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Contests.RankUpAfterContest;

internal sealed class RankUpAfterContestCommandHandler(
    IContestRepository contestRepository,
    IRankUpService rankUpService) : ICommandHandler<RankUpAfterContestCommand>
{
    public async Task<Result> Handle(
        RankUpAfterContestCommand request,
        CancellationToken cancellationToken)
    {
        var standing = await contestRepository
            .GetContestStanding(request.ContestId);

        if (standing is null || standing.Count == 0)
        {
            return Result.Failure(new Error("Contest.Standing.Empty", "No standing found."));
        }

        foreach (var entry in standing)
        {
            var points = RankPointsFromRank((int)entry.Rank, standing.Count);

            await rankUpService.LevelUpUserRank(entry.UserId, points, cancellationToken);
        }

        return Result.Success();
    }


    /// <summary>
    /// Awards points based on rank position relative to total participants.
    /// Top performers get higher ContestPoints levels.
    /// </summary>
    private static ContestPoints RankPointsFromRank(int rank, int total)
    {
        var percentile = (double)rank / total;

        return percentile switch
        {
            <= 0.05 => ContestPoints.Level20, // top 5%
            <= 0.10 => ContestPoints.Level17,
            <= 0.20 => ContestPoints.Level14,
            <= 0.35 => ContestPoints.Level11,
            <= 0.50 => ContestPoints.Level8,
            <= 0.65 => ContestPoints.Level6,
            <= 0.80 => ContestPoints.Level4,
            _ => ContestPoints.Level2   // bottom 20%
        };
    }
}
