using CodeClash.Domain.Premitives;
using MediatR;

namespace CodeClash.Application.Problems.DeleteProblem;
public record DeleteProblemCommand(
    Guid Id) : IRequest<Result>;
