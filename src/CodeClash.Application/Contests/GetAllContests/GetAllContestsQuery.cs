using CodeClash.Application.Abstractions.Messaging;

namespace CodeClash.Application.Contests.GetAllContests;
public record GetAllContestsQuery : IQuery<IReadOnlyList<GetAllContestsResponse>>;
