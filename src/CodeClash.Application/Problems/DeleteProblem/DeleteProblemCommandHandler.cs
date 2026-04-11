using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Problems;
using CodeClash.Domain.Premitives;
using MediatR;

namespace CodeClash.Application.Problems.DeleteProblem;
internal sealed class DeleteProblemCommandHandler(
    IProblemRepository problemRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteProblemCommand, Result>
{
    public async Task<Result> Handle(
        DeleteProblemCommand request,
        CancellationToken cancellationToken)
    {
        var problem = await problemRepository.GetByIdAsync(request.Id);

        if (problem is null)
        {
            return Result.Failure(ProblemErrors.NotFound);
        }

        problemRepository.Delete(problem);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Problem Deleted Successfully");

    }
}
