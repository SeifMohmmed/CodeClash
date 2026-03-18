using CodeClash.Application.Abstractions.Execution;
using CodeClash.Application.SolveProblem.SubmitSolutions;
using CodeClash.Domain.Models.Submits;
using CodeClash.Domain.Premitives;
using Microsoft.AspNetCore.Http;

namespace CodeClash.Application.Mapping;
public static class SubmitMappers
{
    public static async Task<Submit> ToEntityAsync(this SubmitSolutionCommand command)
    {
        return new Submit
        {
            UserId = command.UserId,
            ProblemId = command.ProblemId,

            ContestId = command.ContestId == Guid.Empty
                ? null
                : command.ContestId,

            Code = await ReadFileAsync(command.Code),

            Language = command.Language,
            SubmissionDate = DateTime.Now,

            // default values
            Result = SubmissionResult.Pending,
            SubmitTime = null,
            SubmitMemory = null,
            Error = null
        };
    }

    public static SubmitSolutionCommandResponse ToResponse(
    this Submit submit,
    List<TestCaseRunResult> testCases)
    {
        return new SubmitSolutionCommandResponse
        {
            ProblemId = submit.ProblemId,
            TestCaseRuns = testCases,
            SubmitTime = submit.SubmitTime ?? 0,
            SubmissionResult = submit.Result,
            Error = submit.Error
        };
    }

    private static async Task<string> ReadFileAsync(IFormFile file)
    {
        using var reader = new StreamReader(file.OpenReadStream());
        return await reader.ReadToEndAsync();
    }
}
