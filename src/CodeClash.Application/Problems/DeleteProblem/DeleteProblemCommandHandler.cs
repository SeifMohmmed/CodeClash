using CodeClash.Application.Abstractions.ElasticSearch;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Problems;
using CodeClash.Domain.Premitives;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeClash.Application.Problems.DeleteProblem;

internal sealed class DeleteProblemCommandHandler(
    IProblemRepository problemRepository,
    IUnitOfWork unitOfWork,
    IElasticService elasticService,
    ILogger<DeleteProblemCommandHandler> logger)
    : IRequestHandler<DeleteProblemCommand, Result>
{
    public async Task<Result> Handle(
        DeleteProblemCommand request,
        CancellationToken cancellationToken)
    {
        var problem = await problemRepository
            .GetByIdAsync(request.Id);

        if (problem is null)
        {
            return Result.Failure(ProblemErrors.NotFound);
        }

        problemRepository.Delete(problem);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var deleted = await elasticService
            .DeleteDocumentAsync(request.Id.ToString(), ElasticSearchIndexes.Problems);

        if (!deleted)
        {
            logger.LogWarning(
                "Problem {ProblemId} was deleted from DB but failed to remove from Elasticsearch.",
                request.Id);
        }

        return Result.Success();

    }
}
