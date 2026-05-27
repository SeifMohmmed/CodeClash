using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Mapping;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Submits;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Submissions.GetProblemSubmissions;
internal sealed class GetProblemSubmissionsQueryHandler(
    ISubmissionRepository submissionRepository)
    : IQueryHandler<GetProblemSubmissionsQuery, IReadOnlyList<GetProblemSubmissionsResponse>>
{
    public async Task<Result<IReadOnlyList<GetProblemSubmissionsResponse>>> Handle(
        GetProblemSubmissionsQuery request,
        CancellationToken cancellationToken)
    {
        var submissions = await submissionRepository.GetAllSubmissions(request.ProblemId, request.UserId);

        if (submissions is null || submissions.Count == 0)
        {
            return Result.Failure<IReadOnlyList<GetProblemSubmissionsResponse>>(SubmitErrors.NotFound);
        }

        var mappedSubmissions = submissions.ToResponse().ToList();

        return Result.Success<IReadOnlyList<GetProblemSubmissionsResponse>>(mappedSubmissions);
    }
}
