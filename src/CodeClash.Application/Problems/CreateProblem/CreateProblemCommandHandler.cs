using CodeClash.Application.Mapping;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Problems;
using CodeClash.Domain.Premitives;
using MediatR;

namespace CodeClash.Application.Problems.CreateProblem;
internal sealed class CreateProblemCommandHandler(
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateProblemCommand, Result<Problem>>
{
    public async Task<Result<Problem>> Handle(
        CreateProblemCommand request,
        CancellationToken cancellationToken)
    {
        var repository = unitOfWork.Repository<Problem>();

        // check if problem already exists
        var exists = await repository.AnyAsync(p => p.Name == request.Name);

        if (exists)
        {
            return Result.Failure<Problem>(ProblemErrors.AlreadyExists);
        }

        var mappedProblem = request.ToEntity();

        await repository.AddAsync(mappedProblem);

        await unitOfWork.CompleteAsync();

        return Result.Success(mappedProblem, "Problem added successfully!");
    }
}
