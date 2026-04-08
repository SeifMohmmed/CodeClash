using CodeClash.Domain.Premitives;
using MediatR;

namespace CodeClash.Application.TestCase.DeleteTestcases;
public sealed record DeleteTestcaseCommand(
    Guid TestcaseId) : IRequest<Result>;
