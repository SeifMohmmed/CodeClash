using System.Text.Json;
using CodeClash.Application.Abstractions.Execution;
using CodeClash.Application.Abstractions.File;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.DTO;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Problems;
using CodeClash.Domain.Premitives;
using CodeClash.Domain.Premitives.Responses;

namespace CodeClash.Application.RunCode;
internal sealed class RunCodeCommandHandler(
    IProblemRepository problemRepository,
    IFileService fileService,
    IExecutionService executionService)
   : ICommandHandler<RunCodeCommand, RunCodeResponse>
{
    public async Task<Result<RunCodeResponse>> Handle(
        RunCodeCommand request,
        CancellationToken cancellationToken)
    {
        var problem =
            await problemRepository.GetByIdAsync(request.ProblemId);

        if (problem is null)
        {
            return Result.Failure<RunCodeResponse>(ProblemErrors.NotFound);
        }

        var customTestcaseDto =
            JsonSerializer.Deserialize<List<CustomTestcaseDto>>(request.CustomTestcasesJson)
            ?? throw new ArgumentException("Invalid or empty test cases JSON.", nameof(request));

        var codeContent = await fileService.ReadFile(request.Code);

        var testCasesDtos = customTestcaseDto
            .Select(c => new TestCasesDto { Input = c.Input, Output = c.ExcpectedOutput })
            .ToList();

        var result = await executionService.RunCodeAsync(
            codeContent,
            request.Language,
            testCasesDtos,
            problem.RunTimeLimit,
            (int)problem.MemoryLimit);

        var response = new RunCodeResponse(
            Input: request.CustomTestcasesJson,
            Output: result is WrongAnswerResponse wa ? wa.ActualOutput : result.SubmissionResult.ToString(),
            Passed: result.SubmissionResult == SubmissionResult.Accepted);

        return Result.Success(response, "Testcases run successfully !!");

        //throw new NotImplementedException();

    }
}
