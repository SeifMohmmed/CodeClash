using CodeClash.Application.Abstractions.Messaging;

namespace CodeClash.Application.Problems.CreateProblem;
public record CreateProblemCommand(
    Guid ContestId,
    string Name,
    string Description,
    string Difficulty,
    decimal MemoryLimit,
    decimal RunTimeLimit,
     string SetterId) : ICommand<CreateProblemResponse>;
