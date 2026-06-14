using CodeClash.Application.Abstractions.CurrentUser;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Mapping;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Submits;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Submissions.GetSubmissionData;

internal sealed class GetSubmissionDataQueryHandler(
    ISubmissionRepository submissionRepository,
    ICurrentUserService currentUserService)
    : IQueryHandler<GetSubmissionDataQuery, GetSubmissionDataResponse>
{
    public async Task<Result<GetSubmissionDataResponse>> Handle(
        GetSubmissionDataQuery request,
        CancellationToken cancellationToken)
    {
        var submission = await submissionRepository
            .GetSubmissionIfAuthorized(
                currentUserService.IdentityId!,
                 request.SubmissionId);

        if (submission is null)
        {
            return Result.Failure<GetSubmissionDataResponse>(SubmitErrors.NotFound);
        }

        return Result.Success(submission.ToSubmit());
    }
}
