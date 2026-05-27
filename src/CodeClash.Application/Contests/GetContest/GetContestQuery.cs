using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Domain.Premitives.Responses;

namespace CodeClash.Application.Contests.GetContest;
public sealed record GetContestQuery(
    Guid Id) : IQuery<IReadOnlyList<ContestProblemResponse>>;
