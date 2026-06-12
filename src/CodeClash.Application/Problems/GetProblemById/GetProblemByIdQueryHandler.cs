using CodeClash.Application.Abstractions.CurrentUser;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Mapping;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Problems;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Problems.GetProblemById;

internal sealed class GetProblemByIdQueryHandler(
    IProblemRepository problemRepository,
    ICurrentUserService currentUserService)
    : IQueryHandler<GetProblemByIdQuery, GetProblemByIdResponse>
{
    public async Task<Result<GetProblemByIdResponse>> Handle(
        GetProblemByIdQuery request,
        CancellationToken cancellationToken)
    {
        var problem = await problemRepository
            .GetProblemDetailsAsync(request.ProblemId, cancellationToken);

        if (problem is null)
        {
            return Result.Failure<GetProblemByIdResponse>(ProblemErrors.NotFound);
        }

        // Get total number of accepted submissions for this problem
        var stats =
            await problemRepository.GetProblemStatsAsync(request.ProblemId, cancellationToken);

        // Check if the current user has already solved this 
        var isSolved =
            await problemRepository.CheckUserSolvedProblemAsync(request.ProblemId, currentUserService.IdentityId!, cancellationToken);

        var response = problem.ToDetailsResponse();

        //  Enrich response with additional computed data
        response.Accepted = stats.AcceptedCount;
        response.Submissions = stats.TotalCount;
        response.IsSolved = isSolved;

        return Result.Success(response);
    }
}
