using CodeClash.Application.Abstractions.Messaging;

namespace CodeClash.Application.Problems.GetProblemTestCases;
public record GetProblemTestCase(
    Guid ProblemId) : IQuery<TestCaseResponse>;
