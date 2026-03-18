using CodeClash.Application.Problems.GetProblemTestCases;
using CodeClash.Application.TestCase.CreateTestcases;
using CodeClash.Domain.Models.TestCases;

namespace CodeClash.Application.Mapping;
public static class TestCaseMappings
{
    public static TestCaseResponse ToResponse(
    this Testcase testcase)
    {
        return new TestCaseResponse(
            testcase.Id,
            testcase.Input,
            testcase.Output
        );
    }

    public static Testcase ToEntity(this CreateTestcaseQuery command)
    {
        return new Testcase
        {
            ProblemId = command.ProblemId,
            Input = command.Input,
            Output = command.Output
        };
    }

    public static IEnumerable<TestCaseResponse> ToResponse(
    this IEnumerable<Testcase> testcase)
    {
        return testcase.Select(x => x.ToResponse());
    }
}
