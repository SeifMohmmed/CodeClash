using CodeClash.Application.Abstractions.Cache;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Domain.Abstractions;
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
            return Result.Failure<
                IReadOnlyList<StandingDto>>
                (new Error("Contest.Not.Found", "Contest not found."));
        }

        if (contest.ContestStatus == ContestStatus.Upcoming)
        {
            return Result.Failure<IReadOnlyList<StandingDto>>(
                new Error("Contest.Not.Started", "Contest has not started yet."));
        }

        if (contest.ContestStatus == ContestStatus.Running)
        {
            //retrun the data from cache
            var leaderboard =
              await cacheService.GetContestStanding(
                    request.ContestId,
                    request.Start,
                    request.Stop);

            if (leaderboard is not null)
            {
                return Result.Success(leaderboard, "Contest Standing Fetched Successfully");
            }

            // Cache miss — fall back to DB
        }

        var standing = await contestRepository.GetContestStanding(
            request.ContestId,
            request.Start,
            request.Stop);

        return Result.Success(standing, "Contest Standing Fetched Successfully");

    }
}
