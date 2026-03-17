using CodeClash.Application.Abstractions.Execution;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Mapping;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Contests;
using CodeClash.Domain.Models.Problems;
using CodeClash.Domain.Premitives;
using Microsoft.EntityFrameworkCore;

namespace CodeClash.Application.SolveProblem;
internal sealed class SubmitSolutionCommandHandler(
    IProblemRepository problemRepository,
    IContestRepository contestRepository,
    ISubmitRepository submitRepository,
    IUnitOfWork unitOfWork,
    IExecutionService executionService) : ICommandHandler<SubmitSolutionCommand, SubmitSolutionCommandResponse>
{
    public async Task<Result<SubmitSolutionCommandResponse>> Handle(
        SubmitSolutionCommand request,
        CancellationToken cancellationToken)
    {
        var problem =
            await problemRepository.GetByIdAsync(request.ProblemId);

        if (problem is null)
        {
            return Result.Failure<SubmitSolutionCommandResponse>(ProblemErrors.NotFound);
        }

        var contest =
            await contestRepository.GetByIdAsync(request.ContestId);

        if (contest is null)
        {
            return Result.Failure<SubmitSolutionCommandResponse>(ContestErrors.NotFound);
        }

        var problemTestcases = await
            problemRepository.GetTestCasesByProblemId(request.ProblemId)
            .ToListAsync(cancellationToken);

        string codeContent;

        using (var reader = new StreamReader(request.Code.OpenReadStream()))
        {
            codeContent = await reader.ReadToEndAsync(cancellationToken);
        }

        var executionResult = await executionService.ExecuteCodeAsync(
            codeContent,
            request.Language,
            problemTestcases,
            problem.RunTimeLimit);


        var submission = await request.ToEntityAsync();

        await submitRepository.AddAsync(submission);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var submitResponse = submission.ToResponse(executionResult);

        return Result.Success(submitResponse);
    }
}
