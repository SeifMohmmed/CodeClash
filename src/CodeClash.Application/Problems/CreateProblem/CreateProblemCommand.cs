using CodeClash.Domain.Premitives;
using MediatR;

namespace CodeClash.Application.Problems.CreateProblem;
public record CreateProblemCommand(
    string Name,
    string Description,
    float Rate) : IRequest<Response>;
