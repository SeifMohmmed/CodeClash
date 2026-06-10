using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Domain.Abstractions;

namespace CodeClash.Application.Contests.GetContestStanding;

public sealed record GetContestStandingQuery(
    Guid ContestId,
    int Index,
    int PageSize) : IQuery<IReadOnlyList<StandingDto>>;
