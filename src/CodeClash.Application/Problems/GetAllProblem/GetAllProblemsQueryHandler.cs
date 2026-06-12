using CodeClash.Application.Abstractions.CurrentUser;
using CodeClash.Application.Abstractions.ElasticSearch;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Mapping;
using CodeClash.Application.Problems.GetAll;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Problems.GetAllProblem;

internal sealed class GetAllProblemsQueryHandler(
    IElasticService elasticService,
    ISubmissionRepository submissionRepository,
    ICurrentUserService currentUserService)
    : IQueryHandler<GetAllProblemsQuery, PagedResult<GetAllProblemResponse>>
{
    public async Task<Result<PagedResult<GetAllProblemResponse>>> Handle(
        GetAllProblemsQuery request,
        CancellationToken cancellationToken)
    {
        // fetch once, reuse for both status filter and IsSolved
        var allSubmissions =
             await submissionRepository.GetUserSubmissionsAsync(currentUserService.IdentityId!);

        var (includeIds, excludeIds) = request.Status switch
        {
            ProblemStatus.Solved => (
        allSubmissions
            .Where(s => s.Value == SubmissionResult.Accepted)
            .Select(s => s.Key)
            .ToList(),
        (List<Guid>?)null),

            ProblemStatus.Attempted => (
                allSubmissions
                    .Where(s => s.Value != SubmissionResult.Accepted)
                    .Select(s => s.Key)
                    .ToList(),
                (List<Guid>?)null),

            ProblemStatus.Todo => (
                (List<Guid>?)null,
                allSubmissions.Keys.ToList()),

            _ => ((List<Guid>?)null, (List<Guid>?)null)
        };

        var (problemDocuments, totalPages) = await elasticService.SearchProblemsAsync(
            request.Name,
            request.Topics,
            request.Difficulty,
            request.SortBy,
            request.Order,
            request.PageNumber,
            request.PageSize,
            includeIds,
            excludeIds);

        var responses = problemDocuments
            .Select(problem =>
            {
                var response = problem.ToGetAllResponse();

                response.IsSolved = allSubmissions.TryGetValue(problem.Id, out var result)
                    && result == SubmissionResult.Accepted;

                return response;
            }).ToList();

        return Result.Success(new PagedResult<GetAllProblemResponse>(responses, totalPages));
    }
}
