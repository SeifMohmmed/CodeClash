using CodeClash.Application.Abstractions.Messaging;

namespace CodeClash.Application.Problems.GetProblemById;
public sealed record GetProblemByIdQuery(
    Guid ProblemId) : IQuery<GetProblemByIdResponse>;
