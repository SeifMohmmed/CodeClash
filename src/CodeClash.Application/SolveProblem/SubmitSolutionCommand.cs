using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Domain.Premitives;
using Microsoft.AspNetCore.Http;

namespace CodeClash.Application.SolveProblem;
public record SubmitSolutionCommand(
    string UserId,
    Guid ProblemId,
    IFormFile Code,
    Guid ContestId,
    Language Language) : ICommand<SubmitSolutionCommandResponse>;
