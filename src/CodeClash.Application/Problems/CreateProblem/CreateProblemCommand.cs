using CodeClash.Domain.Models.Problems;
using CodeClash.Domain.Premitives;
using MediatR;

namespace CodeClash.Application.Problems.CreateProblem;
public record CreateProblemCommand(
    string Name,
    string Description,
    float Rate) : IRequest<Result<Problem>>;
