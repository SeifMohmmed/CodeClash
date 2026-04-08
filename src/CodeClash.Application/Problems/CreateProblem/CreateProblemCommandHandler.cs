using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Mapping;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Contests;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Problems.CreateProblem;
internal sealed class CreateProblemCommandHandler(
    IUnitOfWork unitOfWork,
    IContestRepository contestRepository,
    IProblemRepository problemRepository)
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

        return Result.Success(problem.ToResponse());
    }
}
