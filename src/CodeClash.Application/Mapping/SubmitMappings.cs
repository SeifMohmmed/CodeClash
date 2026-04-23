using CodeClash.Application.SolveProblem.SubmitSolutions;
using CodeClash.Application.Submissions.GetProblemSubmissions;
using CodeClash.Application.Submissions.GetSubmissionData;
using CodeClash.Domain.Models.Submits;
using CodeClash.Domain.Premitives;
using CodeClash.Domain.Premitives.Responses;
using Microsoft.AspNetCore.Http;

namespace CodeClash.Application.Mapping;
public static class SubmitMappings
{
    public static GetProblemSubmissionsResponse ToResponse(this Submit submit)
    {
        return new GetProblemSubmissionsResponse
        {
            SubmitTime = submit.SubmitTime ?? 0,
            SubmitMemory = submit.SubmitMemory ?? 0,
            Result = submit.Result,
            Code = submit.Code,
            SubmissionDate = submit.SubmissionDate,
            Language = submit.Language
        };
    }

    public static IEnumerable<GetProblemSubmissionsResponse> ToResponse(
        this IEnumerable<Submit> submits)
    {
        return submits.Select(x => x.ToResponse());
    }

    public static GetSubmissionDataResponse ToSubmit(this Submit submit)
    {
        return new GetSubmissionDataResponse
        {
            Code = submit.Code,
            SubmitTime = submit.SubmitTime ?? 0,       // handle null
            SubmitMemory = submit.SubmitMemory ?? 0,   // handle null
            Language = submit.Language,
            Result = submit.Result,
            SubmissionDate = submit.SubmissionDate,
            Error = submit.Error
        };
    }

    public static async Task<Submit> ToEntityAsync(
        this SubmitSolutionCommand command,
        string userId)
    {
        return new Submit
        {
            UserId = userId,
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
    BaseSubmissionResponse result)
    {
        if (result is CompilationErrorResponse ce)
        {
            return new SubmitSolutionCommandResponse
            {
                ProblemId = submit.ProblemId,
                SubmitTime = submit.SubmitTime ?? 0,
                SubmissionResult = result.SubmissionResult,

                Error = result is RunTimeErrorResponse rte ? rte.Message :
                ce.Message
            };
        }
        else
        {
            return new SubmitSolutionCommandResponse
            {
                ProblemId = submit.ProblemId,
                SubmitTime = submit.SubmitTime ?? 0,
                SubmissionResult = result.SubmissionResult,

                Error = result is RunTimeErrorResponse rte ? rte.Message :
                null
            };
        }
    }

    private static async Task<string> ReadFileAsync(IFormFile file)
    {
        using var reader = new StreamReader(file.OpenReadStream());
        return await reader.ReadToEndAsync();
    }
}
