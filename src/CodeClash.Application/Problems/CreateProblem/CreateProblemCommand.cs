using CodeClash.Application.Abstractions.Messaging;

namespace CodeClash.Application.Problems.CreateProblem;
public record CreateProblemCommand(
    Guid ContestId,
    string ProblemSetterId,
    string Difficulty,
    decimal RunTimeLimit,
    decimal MemoryLimit) : ICommand<Guid>;
