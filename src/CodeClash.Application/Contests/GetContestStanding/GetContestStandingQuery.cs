using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Domain.Abstractions;

namespace CodeClash.Application.Contests.GetContestStanding;

public sealed record GetContestStandingQuery(
    Guid ContestId,
    int Start = 0,
    int Stop = 20) : IQuery<IReadOnlyList<StandingDto>>;
