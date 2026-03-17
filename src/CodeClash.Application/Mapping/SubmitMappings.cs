using CodeClash.Application.Submissions.GetProblemSubmissions;
using CodeClash.Domain.Models.Submits;

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

}
