using CodeClash.Application.Mapping;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Premitives;
using MediatR;

namespace CodeClash.Application.TestCase.UpdateTestcases;
internal sealed class UpdateTestcaseCommandHandler(
    ITestCaseRepository testCaseRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateTestcaseCommand, Result>
{
    public async Task<Result> Handle(
        UpdateTestcaseCommand request,
        CancellationToken cancellationToken)
    {
        var existingTestcase =
            await testCaseRepository.GetByIdAsync(request.TestcaseId);

        if (existingTestcase is null)
        {
            return Result.Failure(new Error("Testcase.NotFound", "Test case was not found."));
        }

        existingTestcase.ApplyUpdate(request);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
