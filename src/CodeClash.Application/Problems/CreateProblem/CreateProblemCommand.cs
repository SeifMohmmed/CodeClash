using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Problems.CreateProblem;
public record CreateProblemCommand(
    Guid ContestId,
    string Name,
    string Description,
    Difficulty Difficulty,
    MemoryLimit MemoryLimit,
    decimal RunTimeLimit,
    List<Guid> Topics,
     string SetterId) : ICommand<CreateProblemResponse>;
