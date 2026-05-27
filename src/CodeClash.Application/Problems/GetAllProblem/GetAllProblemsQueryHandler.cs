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
    : IQueryHandler<GetAllProblemsQuery, PagedResult<GetAllProblemResponse>>
{
    public async Task<Result<PagedResult<GetAllProblemResponse>>> Handle(
        GetAllProblemsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
        {
            return Result.Failure<PagedResult<GetAllProblemResponse>>(new Error("Auth.Error", "Unauthorized"));
        }

        var (problemDocuments, totalPages) = await elasticService.SearchProblemsAsync(
            request.Name,
            request.Topics,
            request.Difficulty,
            request.SortBy,
            request.Order,
            request.PageNumber,
            request.PageSize);

        var problemList = problemDocuments.ToList() ?? [];

        // fetch once, reuse for both status filter and IsSolved
        var allSubmissions =
             await submissionRepository.GetUserSubmissionsAsync(userId);

        if (request.Status is not null)
        {
            problemList = request.Status switch
            {
                ProblemStatus.Solved => problemList
                    .Where(p => allSubmissions.TryGetValue(p.Id, out var r) && r == SubmissionResult.Accepted)
                    .ToList(),

                ProblemStatus.Attempted => problemList
                    .Where(p => allSubmissions.TryGetValue(p.Id, out var r) && r != SubmissionResult.Accepted)
                    .ToList(),

                ProblemStatus.Todo => problemList
                    .Where(p => !allSubmissions.ContainsKey(p.Id))
                    .ToList(),

                _ => problemList
            };
        }

        var responses = problemList.Select(problem =>
        {
            var response = problem.ToGetAllResponse();
            response.IsSolved = allSubmissions.TryGetValue(problem.Id, out var result)
                && result == SubmissionResult.Accepted;
            return response;
        }).ToList();

        return Result.Success(new PagedResult<GetAllProblemResponse>(responses, totalPages));
    }
}
