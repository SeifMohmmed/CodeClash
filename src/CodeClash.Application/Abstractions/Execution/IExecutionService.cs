using CodeClash.Domain.Models.TestCases;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Abstractions.Execution;
/// <summary>
/// Abstraction for code execution service.
/// Responsible for running user code against test cases.
/// </summary>
public interface IExecutionService
{
    /// <summary>
    /// Executes user code with given language and test cases.
    /// </summary>
    Task<List<TestCaseRunResult>> ExecuteCodeAsync(string code, Language language, List<Testcase> testCases, decimal runTimeLimit);
}
