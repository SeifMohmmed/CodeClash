namespace CodeClash.API.Controllers.Testcases;

public sealed record CreateTestcaseRequest
{
    public required Guid ProblemId { get; init; }
    public required string Input { get; init; }
    public required string Output { get; init; }
}
