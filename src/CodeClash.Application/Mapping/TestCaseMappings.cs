using CodeClash.Application.Problems.GetPrblemTestcases;
using CodeClash.Application.TestCase.CreateTestcases;
using CodeClash.Application.TestCase.UpdateTestcases;
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

    public static Testcase ToEntity(this CreateTestcaseCommand command)
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

    public static void ApplyUpdate(
        this Testcase testcase,
        UpdateTestcaseCommand command)
    {
        testcase.Input = command.Input;
        testcase.Output = command.Output;
    }
}
