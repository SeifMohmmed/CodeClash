using CodeClash.Application.Abstractions.ElasticSearch;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Mapping;
using CodeClash.Application.Problems.GetAll;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Problems;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Problems.GetAllProblem;
internal sealed class GetAllProblemsQueryHandler(
    IElasticService elasticService,
    ISubmissionRepository submissionRepository)
    : IQueryHandler<GetAllProblemsQuery, IEnumerable<GetAllProblemResponse>>
{
    public async Task<Result<IEnumerable<GetAllProblemResponse>>> Handle(
        GetAllProblemsQuery request,
        CancellationToken cancellationToken)
    {
        var problems = await elasticService
            .SearchProblemsAsync(request.Name, request.TopicsIds, (int?)request.Difficulty ?? 0);

        var problemList = problems?.ToList() ?? [];
        if (!problemList.Any())
        {
            return Result.Failure<IEnumerable<GetAllProblemResponse>>(ProblemErrors.NotFound);
        }

        // Fetch all solved problem IDs in one query
        var problemIds = problemList.Select(p => p.Id).ToList();
        var solvedIds = await submissionRepository
            .GetSolvedProblemIdsAsync(problemIds, request.UserId!);

        var responses = problemList.Select(problem =>
        {
            var result = problem.ToGetAllResponse();
            result.IsSolved = solvedIds.Contains(problem.Id);
            return result;
        }).ToList();

        return Result.Success<IEnumerable<GetAllProblemResponse>>(responses);
    }
}
