using CodeClash.Domain.Premitives;
using MediatR;

namespace CodeClash.Application.TestCase.UpdateTestcases;
public sealed record UpdateTestcaseCommand(
    Guid TestcaseId,
    string Input,
    string Output) : IRequest<Result>;
