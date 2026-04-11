using CodeClash.Domain.Premitives;

namespace CodeClash.API.Controllers.Problems;

public sealed class GetAllProblemsRequest
{
    public string? Name { get; init; }
    public List<int> TopicsIds { get; init; } = [];
    public Difficulty? Difficulty { get; init; }
}
