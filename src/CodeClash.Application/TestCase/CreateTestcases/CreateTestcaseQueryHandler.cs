using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Mapping;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Problems;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.TestCase.CreateTestcases;
internal sealed class CreateTestcaseQueryHandler(
    IUnitOfWork unitOfWork,
    ITestCaseRepository testCaseRepository,
    IProblemRepository problemRepository)
    : ICommandHandler<CreateTestcaseQuery, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateTestcaseQuery request,
        CancellationToken cancellationToken)
    {
        var problem = await problemRepository.GetByIdAsync(request.ProblemId);

        if (problem is null)
        {
            return Result.Failure<Guid>(ProblemErrors.NotFound);
        }

        var testcase = request.ToEntity();

        await testCaseRepository.AddAsync(testcase);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(testcase.Id);
    }
}
