using System.Security.Claims;
using CodeClash.Application.Abstractions.Cache;
using CodeClash.Application.Abstractions.Execution;
using CodeClash.Application.Abstractions.File;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Mapping;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Contests;
using CodeClash.Domain.Models.Problems;
using CodeClash.Domain.Premitives;
using CodeClash.Domain.Premitives.Responses;
using CodeClash.Domain.Requests;
using Microsoft.AspNetCore.Http;

namespace CodeClash.Application.SolveProblem.SubmitSolutions;

internal sealed class SubmitSolutionCommandHandler(
    IProblemRepository problemRepository,
    ISubmitRepository submitRepository,
    IUnitOfWork unitOfWork,
    IExecutionService executionService,
    ICacheService cacheService,
    IHttpContextAccessor contextAccessor,
    IFileService fileService)
    : ICommandHandler<SubmitSolutionCommand, SubmitSolutionCommandResponse>
{
    public async Task<Result<SubmitSolutionCommandResponse>> Handle(
        SubmitSolutionCommand request,
        CancellationToken cancellationToken)
    {
        // Auth
        var userId = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            return Result.Failure<SubmitSolutionCommandResponse>(new Error("Auth.Unauthorized", "Unauthorized"));
        }

        // Load problem with testcases
        var problem =
            await problemRepository.GetProblemIncludingContestAndTestcases(request.ProblemId, cancellationToken);
        if (problem is null)
        {
            return Result.Failure<SubmitSolutionCommandResponse>
                (ProblemErrors.NotFound);
        }

        // Contest validation
        if (request.ContestId.HasValue && problem.Contest is null)
        {
            return Result.Failure<SubmitSolutionCommandResponse>(ContestErrors.NotFound);
        }

        // Contest status check
        if (problem.Contest?.ContestStatus == ContestStatus.Upcoming)
        {
            return Result.Failure<SubmitSolutionCommandResponse>(ContestErrors.NotStarted);
        }

        if (problem.Contest?.ContestStatus == ContestStatus.Ended)
        {
            return Result.Failure<SubmitSolutionCommandResponse>(ContestErrors.Ended);
        }

        // Read code file
        string codeContent = await fileService.ReadFile(request.Code);

        // Execute
        var executionResult = await executionService.RunCodeAsync(
            codeContent,
            request.Language,
            problem.Testcases.ToList(),
            problem.RunTimeLimit);

        // Build submission entity
        var submission = request.ToEntity(userId, codeContent);
        submission.Result = executionResult.SubmissionResult;
        submission.SubmitTime = (executionResult as AcceptedResponse)?.ExecutionTime;
        submission.Error = (executionResult as CompilationErrorResponse)?.Message;
        submission.SubmitMemory = 0m;

        // Persist
        submitRepository.Add(submission);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Update standings if accepted in running contest
        if (problem.Contest?.ContestStatus == ContestStatus.Running)
        {
            // Check caching the submission
            bool isFirstSolve = executionResult.SubmissionResult == SubmissionResult.Accepted
                && !await cacheService.IsUserSolvedTheProblemAsync(userId, problem.Contest.Id, problem.Id);

            // Always cache the submission
            await cacheService.CacheUserSubmissionAsync(new SubmissionToCache
            {
                Date = submission.SubmissionDate,
                Language = submission.Language,
                ProblemId = submission.ProblemId,
                Result = submission.Result
            }, userId, problem.Contest.Id);

            // Only update standing on first accepted solve
            if (isFirstSolve)
            {
                var claimsPrincipal = contextAccessor.HttpContext!.User;

                await cacheService.CacheContestStandingAsync(
                    problem.ContestPoints,
                    new UserToCache
                    {
                        UserId = userId,
                        UserName = claimsPrincipal.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
                        UserImage = claimsPrincipal.FindFirstValue("ImagePath")
                    },
                    problem.Contest.Id);
            }
        }

        return Result.Success(submission.ToResponse(executionResult));
    }
}
