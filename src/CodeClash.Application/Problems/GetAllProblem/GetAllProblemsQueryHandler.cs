using System.Security.Claims;
using CodeClash.Application.Abstractions.ElasticSearch;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Mapping;
using CodeClash.Application.Problems.GetAll;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Premitives;
using Microsoft.AspNetCore.Http;

namespace CodeClash.Application.Problems.GetAllProblem;
internal sealed class GetAllProblemsQueryHandler(
    IElasticService elasticService,
    ISubmissionRepository submissionRepository,
    IHttpContextAccessor contextAccessor)
    : IQueryHandler<GetAllProblemsQuery, IEnumerable<GetAllProblemResponse>>
{
    public async Task<Result<IEnumerable<GetAllProblemResponse>>> Handle(
        GetAllProblemsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
        {
            return Result.Failure<IEnumerable<GetAllProblemResponse>>(new Error("Auth.Error", "Unauthorized"));
        }

        var problems = await elasticService
            .SearchProblemsAsync(request.Name, request.TopicsIds, request.Difficulty, request.PageNumber, request.PageSize);

        var problemList = problems?.ToList() ?? [];

        var submissions = await submissionRepository
            .GetUserAcceptedSubmissions(userId);

        var responses = problemList.Select(problem =>
        {
            var result = problem.ToGetAllResponse();
            result.IsSolved = submissions.Contains(problem.Id);
            return result;
        }).ToList();

        return Result.Success<IEnumerable<GetAllProblemResponse>>(responses);
    }
}
