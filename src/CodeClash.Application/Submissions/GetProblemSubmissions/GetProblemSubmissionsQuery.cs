using CodeClash.Application.Abstractions.Messaging;

namespace CodeClash.Application.Submissions.GetProblemSubmissions;
public sealed record GetProblemSubmissionsQuery(
     Guid ProblemId,
     string UserId) : IQuery<IReadOnlyList<GetProblemSubmissionsResponse>>;
