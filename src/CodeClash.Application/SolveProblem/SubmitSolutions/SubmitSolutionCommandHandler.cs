using System.Security.Claims;
using CodeClash.Application.Abstractions.Execution;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.DTO;
using CodeClash.Application.Mapping;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Contests;
using CodeClash.Domain.Models.Problems;
using CodeClash.Domain.Premitives;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CodeClash.Application.SolveProblem.SubmitSolutions;
internal sealed class SubmitSolutionCommandHandler(
    IProblemRepository problemRepository,
    IContestRepository contestRepository,
    ISubmitRepository submitRepository,
    IUnitOfWork unitOfWork,
    IExecutionService executionService,
    IHttpContextAccessor contextAccessor) : ICommandHandler<SubmitSolutionCommand, SubmitSolutionCommandResponse>
{
    public async Task<Result<SubmitSolutionCommandResponse>> Handle(
        SubmitSolutionCommand request,
        CancellationToken cancellationToken)
    {
        var userId = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
        {
            return Result.Failure<SubmitSolutionCommandResponse>(new Error("Auth.Error", "Unauthorized"));
        }

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

        var testCasesDtos = problemTestcases
        .Select(t => new TestCasesDto { Input = t.Input, Output = t.Output })
        .ToList();

        var executionResult = await executionService.RunCodeAsync(
            codeContent,
            request.Language,
            testCasesDtos,
            problem.RunTimeLimit,
            problem.MemoryLimit);

        var submission = await request.ToEntityAsync(userId);

        submitRepository.Add(submission);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var submitResponse = submission.ToResponse(executionResult);

        return Result.Success(submitResponse);
    }
}
