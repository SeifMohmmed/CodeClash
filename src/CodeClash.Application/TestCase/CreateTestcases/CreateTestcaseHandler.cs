using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Problems;
using CodeClash.Domain.Models.TestCases;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.TestCase.CreateTestcases;
internal sealed class CreateTestcaseHandler(
    IUnitOfWork unitOfWork,
    ITestCaseRepository testCaseRepository,
    IProblemRepository problemRepository)
    : ICommandHandler<CreateTestcase, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateTestcase request,
        CancellationToken cancellationToken)
    {
        var problem = await problemRepository.GetByIdAsync(request.ProblemId);

        if (problem is null)
        {
            return Result.Failure<Guid>(ProblemErrors.NotFound);
        }

        var testcase = new Testcase
        {
            ProblemId = request.ProblemId,
            Input = request.Input,
            Output = request.Output
        };

        await testCaseRepository.AddAsync(testcase);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(testcase.Id);
    }
}
