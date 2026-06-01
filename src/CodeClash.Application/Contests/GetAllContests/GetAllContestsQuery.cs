using CodeClash.Application.Abstractions.Messaging;

namespace CodeClash.Application.Contests.GetAllContests;

public record GetAllContestsQuery(
    string UserId) : IQuery<IReadOnlyList<GetAllContestsResponse>>;
