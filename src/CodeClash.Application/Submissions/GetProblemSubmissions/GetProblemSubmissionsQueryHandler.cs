using CodeClash.Application.Abstractions.CurrentUser;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Mapping;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Submissions.GetProblemSubmissions;

internal sealed class GetProblemSubmissionsQueryHandler(
    ISubmissionRepository submissionRepository,
    ICurrentUserService currentUserService)
    : IQueryHandler<GetProblemSubmissionsQuery, IReadOnlyList<GetProblemSubmissionsResponse>>
{
    public async Task<Result<IReadOnlyList<GetProblemSubmissionsResponse>>> Handle(
        GetProblemSubmissionsQuery request,
        CancellationToken cancellationToken)
    {
        var submissions = await submissionRepository
            .GetAllSubmissions(request.ProblemId, currentUserService.IdentityId!);

        return Result.Success<IReadOnlyList<GetProblemSubmissionsResponse>>(
            submissions.ToResponse().ToList());
    }
}
