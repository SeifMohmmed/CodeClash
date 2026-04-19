using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Domain.Premitives;
using Microsoft.AspNetCore.Http;

namespace CodeClash.Application.RunCode;
internal sealed record RunCodeCommand(
    Language Language,
    IFormFile Code,
    Guid ProblemId,
    string CustomTestcasesJson) : ICommand<RunCodeResponse>;
