using System.Security.Claims;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Mapping;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Problems;
using CodeClash.Domain.Models.Submits;
using CodeClash.Domain.Premitives;
using Microsoft.AspNetCore.Http;

namespace CodeClash.Application.Submissions.GetProblemSubmissions;
internal sealed class GetProblemSubmissionsQueryHandler(
    IProblemRepository problemRepository,
    ISubmissionRepository submissionRepository,
    IHttpContextAccessor contextAccessor)
    : IQueryHandler<GetProblemSubmissionsQuery, IReadOnlyList<GetProblemSubmissionsResponse>>
{
    public async Task<Result<IReadOnlyList<GetProblemSubmissionsResponse>>> Handle(
        GetProblemSubmissionsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
        {
            return Result.Failure<IReadOnlyList<GetProblemSubmissionsResponse>>(new Error("Auth.Error", "Unauthorized"));
        }

        var problem = await problemRepository.GetByIdAsync(request.ProblemId);

        if (problem is null)
        {
            return Result.Failure<IReadOnlyList<GetProblemSubmissionsResponse>>(ProblemErrors.NotFound);
        }

        var submissions = submissionRepository.GetAllSubmissions(request.ProblemId, userId);

        if (submissions is null)
        {
            return Result.Failure<IReadOnlyList<GetProblemSubmissionsResponse>>(SubmitErrors.NotFound);
        }

        var mappedSubmissions = submissions.ToResponse().ToList();

        return Result.Success<IReadOnlyList<GetProblemSubmissionsResponse>>(mappedSubmissions);
    }
}
