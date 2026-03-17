using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Mapping;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Problems.CreateProblem;
internal sealed class CreateProblemCommandHandler(
    IUnitOfWork unitOfWork,
    IProblemRepository problemRepository)
    : ICommandHandler<CreateProblemCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateProblemCommand request,
        CancellationToken cancellationToken)
    {
        var mappedProblem = request.ToEntity();

        await problemRepository.AddAsync(mappedProblem);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(mappedProblem.Id);
    }
}
