using CodeClash.Application.Abstractions.Cache;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Contests;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Contests.GetContestStanding;

internal sealed class GetContestStandingQueryHandler(
    IContestRepository contestRepository,
    ICacheService cacheService) : IQueryHandler<GetContestStandingQuery, IReadOnlyList<StandingDto>>
{
    public async Task<Result<IReadOnlyList<StandingDto>>> Handle(
        GetContestStandingQuery request,
        CancellationToken cancellationToken)
    {
        var contest = await contestRepository.GetByIdAsync(request.ContestId);

        if (contest is null)
        {
            return Result.Failure<IReadOnlyList<StandingDto>>(ContestErrors.NotFound);
        }

        if (contest.ContestStatus == ContestStatus.Upcoming)
        {
            return Result.Failure<IReadOnlyList<StandingDto>>(ContestErrors.NotStarted);
        }

        if (contest.ContestStatus == ContestStatus.Running)
        {
            //retrun the data from cache
            var leaderboard = await cacheService.GetContestStanding(
                    request.ContestId,
                    request.Start,
                    request.Stop);

            // During a running contest standings are Redis-only —
            // no DB fallback since live submissions aren't persisted to a standings table.
            return Result.Success<IReadOnlyList<StandingDto>>(leaderboard);
        }

        // Ended contest — read from DB
        var standing = await contestRepository.GetContestStanding(
            request.ContestId,
            request.Start,
            request.Stop);

        return Result.Success<IReadOnlyList<StandingDto>>(standing);

    }
}
