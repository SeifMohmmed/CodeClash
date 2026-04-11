using CodeClash.Domain.Premitives;

namespace CodeClash.API.Controllers.Problems;
public sealed record CreateProblemRequest
{
    public required Guid ContestId { get; init; }
    public required string Name { get; init; }
    public required string SetterId { get; init; }
    public required string Description { get; init; }
    public required Difficulty Difficulty { get; init; }
    public required MemoryLimit MemoryLimit { get; init; }
    public required int RunTimeLimit { get; init; }
    public required List<Guid> Topics { get; init; }
}
