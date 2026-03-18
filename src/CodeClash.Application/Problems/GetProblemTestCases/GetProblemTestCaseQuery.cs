using CodeClash.Application.Abstractions.Messaging;

namespace CodeClash.Application.Problems.GetProblemTestCases;
public record GetProblemTestCaseQuery(
    Guid ProblemId) : IQuery<List<TestCaseResponse>>;
