using CodeClash.Application.Abstractions.ElasticSearch;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Mapping;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Contests;
using CodeClash.Domain.Premitives;
using CodeClash.Domain.Premitives.Responses.ElasticSearchResponses;

namespace CodeClash.Application.Problems.CreateProblem;
internal sealed class CreateProblemCommandHandler(
    IUnitOfWork unitOfWork,
    IContestRepository contestRepository,
    IProblemRepository problemRepository,
    IElasticService elasticService)
    : ICommandHandler<CreateProblemCommand, CreateProblemResponse>
{
    public async Task<Result<CreateProblemResponse>> Handle(
        CreateProblemCommand request,
        CancellationToken cancellationToken)
    {
        var contest =
            await contestRepository.GetByIdAsync(request.ContestId);

        if (contest is null)
        {
            return Result.Failure<CreateProblemResponse>(ContestErrors.NotFound);
        }

        var problem = request.ToEntity();

        problemRepository.Add(problem);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var document = new ProblemDocument
        {
            Difficulty = problem.Difficulty,
            Id = problem.Id,
            Name = problem.Name,
            Topics = new List<string>()
        };

        var result = await elasticService.IndexDocumentAsync(document, ElasticSearchIndexes.Problems);

        if (!result)
        {
            return Result.Failure<CreateProblemResponse>(new Error("Error", "failed to index problem"));
        }

        return Result.Success(problem.ToResponse());
    }
}
