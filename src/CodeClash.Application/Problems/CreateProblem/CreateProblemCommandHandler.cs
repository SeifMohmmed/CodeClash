using CodeClash.Application.Abstractions.ElasticSearch;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Mapping;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Contests;
using CodeClash.Domain.Models.Topics;
using CodeClash.Domain.Premitives;
using CodeClash.Domain.Premitives.Responses.ElasticSearchResponses;
using Microsoft.Extensions.Logging;

namespace CodeClash.Application.Problems.CreateProblem;

internal sealed class CreateProblemCommandHandler(
    IUnitOfWork unitOfWork,
    IContestRepository contestRepository,
    IProblemRepository problemRepository,
    ITopicRepository topicRepository,
    IElasticService elasticService,
    ILogger<CreateProblemCommandHandler> logger)
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

        var existingTopicIds =
            await topicRepository.GetExistingIdsAsync(request.Topics, cancellationToken);

        var missingTopicId = request.Topics
            .Cast<Guid?>()
            .FirstOrDefault(id => !existingTopicIds.Contains(id!.Value));

        if (missingTopicId is not null)
        {
            return Result.Failure<CreateProblemResponse>(TopicErrors.NotFound(missingTopicId.Value));
        }

        var problem = request.ToEntity();

        problemRepository.Add(problem);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var document = new ProblemDocument
        {
            Id = problem.Id,
            Name = problem.Name,
            Difficulty = problem.Difficulty,
            Topics = request.Topics
        };

        var indexed = await elasticService.IndexDocumentAsync(document, ElasticSearchIndexes.Problems);

        if (!indexed)
        {
            logger.LogWarning(
                 "Problem {ProblemId} was persisted but failed to index in Elasticsearch. Manual or background re-sync required.",
                 problem.Id);
        }

        return Result.Success(problem.ToResponse(), "Problem Created Successfully!");
    }
}
