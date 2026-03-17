using CodeClash.Application.Abstractions.Messaging;

namespace CodeClash.Application.Submissions.GetProblemSubmissions;
public sealed record GetProblemSubmissions(
     Guid ProblemId
 ) : IQuery<IReadOnlyList<GetProblemSubmissionsResponse>>;
