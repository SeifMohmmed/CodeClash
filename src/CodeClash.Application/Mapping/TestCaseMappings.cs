using CodeClash.Application.Problems.GetProblemTestCases;
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

    public static IEnumerable<TestCaseResponse> ToResponse(
    this IEnumerable<Testcase> testcase)
    {
        return testcase.Select(x => x.ToResponse());
    }
}
