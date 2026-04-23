using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Domain.Premitives;
using Microsoft.AspNetCore.Http;

namespace CodeClash.Application.SolveProblem.SubmitSolutions;
public record SubmitSolutionCommand(
    Guid ProblemId,
    IFormFile Code,
    Guid ContestId,
    Language Language) : ICommand<SubmitSolutionCommandResponse>;
