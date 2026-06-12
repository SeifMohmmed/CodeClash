using CodeClash.Application.Abstractions.Messaging;

namespace CodeClash.Application.Problems.GetPrblemTestcases;
public record GetTestCaseQuery(
    Guid ProblemId) : IQuery<List<TestCaseResponse>>;
