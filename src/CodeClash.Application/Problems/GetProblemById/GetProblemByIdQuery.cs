using CodeClash.Application.Abstractions.Messaging;

namespace CodeClash.Application.Problems.GetProblemById;
public sealed record GetProblemByIdQuery(
    Guid ProblemId,
    string UserId) : IQuery<GetProblemByIdResponse>;
