using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Problems.GetPrblemTestcases;

internal sealed class GetProblemTestcasesQueryHandler(
    IProblemRepository problemRepository)
    : IQueryHandler<GetTestCaseQuery, List<TestCaseResponse>>
{
    public async Task<Result<List<TestCaseResponse>>> Handle(
        GetTestCaseQuery request,
        CancellationToken cancellationToken)
    {
        var problem = await problemRepository.GetByIdAsync(request.ProblemId);

        if (problem is null)
        {
            return Result.Failure<List<TestCaseResponse>>(
                new Error("Problem.NotFound", "Problem was not found."));
        }

        var testCases = await problemRepository.GetTestCasesByProblemIdAsync(request.ProblemId, cancellationToken);

        return testCases
            .Select(tc => new TestCaseResponse(tc.Input, tc.Output))
            .ToList();
    }
}
