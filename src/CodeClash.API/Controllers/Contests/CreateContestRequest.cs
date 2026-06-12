namespace CodeClash.API.Controllers.Contests;

public sealed record CreateContestRequest
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required DateTime StartTime { get; init; }
    public required DateTime EndTime { get; init; }
}
