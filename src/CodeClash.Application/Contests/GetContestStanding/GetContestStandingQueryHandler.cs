using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Contests.GetContestStanding;

internal sealed class GetContestStandingQueryHandler(
    IContestRepository contestRepository) : IQueryHandler<GetContestStandingQuery, IReadOnlyList<StandingDto>>
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

        var standing = await contestRepository.GetContestStanding(request.ContestId);

        return Result.Success(standing, "Contest Standing Fetched Successfully");

    }
}
