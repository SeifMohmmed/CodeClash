using CodeClash.Application.Abstractions.Messaging;

namespace CodeClash.Application.Problems.CreateProblem;
public record CreateProblemCommand(
    string Name,
    string Description,
    float Rate) : ICommand<Guid>;
