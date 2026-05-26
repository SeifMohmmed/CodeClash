using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Domain.Premitives.Responses;

namespace CodeClash.Application.Contest.GetContest;
public sealed record GetContestQuery(
    Guid Id) : IQuery<IReadOnlyList<ContestProblemResponse>>;
