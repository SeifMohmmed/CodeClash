using CodeClash.Application.DTO;
using CodeClash.Domain.Models.TestCases;

namespace CodeClash.Application.Mapping;
internal static class TestcaseMapping
{
    public static TestCasesDto ToDto(Testcase testcase)
    {
        return new TestCasesDto
        {
            Input = testcase.Input,
            Output = testcase.Output
        };
    }
}
