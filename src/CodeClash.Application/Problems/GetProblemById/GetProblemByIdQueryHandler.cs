using System.Security.Claims;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Mapping;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Problems;
using CodeClash.Domain.Premitives;
using Microsoft.AspNetCore.Http;

namespace CodeClash.Application.Problems.GetProblemById;
internal sealed class GetProblemByIdQueryHandler(
    IProblemRepository problemRepository,
    IHttpContextAccessor contextAccessor)
    : IQueryHandler<GetProblemByIdQuery, GetProblemByIdResponse>
{
    public async Task<Result<GetProblemByIdResponse>> Handle(
        GetProblemByIdQuery request,
        CancellationToken cancellationToken)
    {
        var userId = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
        {
            return Result.Failure<GetProblemByIdResponse>(new Error("Auth.Error", "Unauthorized"));
        }

        var problem = await problemRepository
            .GetProblemDetailsAsync(request.ProblemId, cancellationToken);

        if (problem is null)
        {
            return Result.Failure<GetProblemByIdResponse>(ProblemErrors.NotFound);
        }

        // Get total number of accepted submissions for this problem
        var accepted =
            await problemRepository.GetAcceptedProblemCountAsync(request.ProblemId, cancellationToken);

        // Get total number of submissions for this problem
        var submissions =
            await problemRepository.GetSubmissionsProblemCountAsync(request.ProblemId, cancellationToken);

        // Check if the current user has already solved this 
        var isSolved =
            await problemRepository.CheckUserSolvedProblemAsync(request.ProblemId, userId, cancellationToken);

        var response = problem.ToDetailsResponse();

        //  Enrich response with additional computed data
        response.Accepted = accepted;
        response.Submissions = submissions;
        response.IsSolved = isSolved;

        return Result.Success(response);
    }
}
