using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Premitives;
using MediatR;

namespace CodeClash.Application.TestCase.DeleteTestcases;
internal sealed class DeleteTestcaseCommandHandler(
    IUnitOfWork unitOfWork,
    ITestCaseRepository testCaseRepository) : IRequestHandler<DeleteTestcaseCommand, Result>
{
    public async Task<Result> Handle(
        DeleteTestcaseCommand request,
        CancellationToken cancellationToken)
    {
        var testcase = await testCaseRepository.GetByIdAsync(request.TestcaseId);

        if (testcase is null)
        {
            return Result.Failure(new Error("Testcase.NotFound", "Test case was not found."));
        }

        testCaseRepository.Delete(testcase);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Test case deleted successfully.");
    }
}
