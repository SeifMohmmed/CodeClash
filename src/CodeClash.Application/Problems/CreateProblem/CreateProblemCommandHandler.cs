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
    : ICommandHandler<CreateProblemCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateProblemCommand request,
        CancellationToken cancellationToken)
    {
        var contest =
            await contestRepository.GetByIdAsync(request.ContestId);

        if (contest is null)
        {
            return Result.Failure<Guid>(ContestErrors.NotFound);
        }

        var mappedProblem = request.ToEntity();

        await problemRepository.AddAsync(mappedProblem);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(mappedProblem.Id);
    }
}
