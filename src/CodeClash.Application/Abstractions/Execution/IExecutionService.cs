using CodeClash.Application.DTO;
using CodeClash.Domain.Models.TestCases;
using CodeClash.Domain.Premitives;
using CodeClash.Domain.Premitives.Responses;

namespace CodeClash.Application.Abstractions.Execution;
/// <summary>
/// Abstraction for code execution service.
/// Responsible for running user code against test cases.
/// </summary>
public interface IExecutionService
{
    Task<BaseSubmissionResponse> RunCodeAsync(
    string code,
    Language language,
    List<Testcase> testCases,
    decimal runTimeLimit);

    /// <summary>
    /// Executes user code with given language and test cases.
    /// </summary>
    Task<BaseSubmissionResponse> RunCodeAsync(
        string code,
        Language language,
        List<TestCasesDto> testCases,
        decimal runTimeLimit);
}
