using System.Security.Claims;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Mapping;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Submits;
using CodeClash.Domain.Premitives;
using Microsoft.AspNetCore.Http;

namespace CodeClash.Application.Submissions.GetSubmissionData;
internal sealed class GetSubmissionDataQueryHandler(
    ISubmissionRepository submissionRepository,
    IHttpContextAccessor contextAccessor)
    : IQueryHandler<GetSubmissionDataQuery, GetSubmissionDataResponse>
{
    public async Task<Result<GetSubmissionDataResponse>> Handle(
        GetSubmissionDataQuery request,
        CancellationToken cancellationToken)
    {
        var userId = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
        {
            return Result.Failure<GetSubmissionDataResponse>(new Error("Auth.Error", "Unauthorized"));
        }

        var submission = await submissionRepository.GetByIdAsync(request.SubmissionId);

        if (submission is null)
        {
            return Result.Failure<GetSubmissionDataResponse>(SubmitErrors.NotFound);
        }

        var mappedSub = submission.ToSubmit();

        return Result.Success(mappedSub);
    }
}
