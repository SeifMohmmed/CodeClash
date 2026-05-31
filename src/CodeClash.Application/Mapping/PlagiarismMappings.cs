using CodeClash.Application.DTO;
using CodeClash.Domain.Models.Submits;

namespace CodeClash.Application.Mapping;

public static class PlagiarismMappings
{
    public static SubmitDto ToDto(this Submit submit) => new()
    {
        UserId = submit.UserId,
        ProblemId = submit.ProblemId,
        ContestId = submit.ContestId,
        Code = submit.Code,
        Language = submit.Language,
        SubmissionDate = submit.SubmissionDate,
        Result = submit.Result
    };
}
