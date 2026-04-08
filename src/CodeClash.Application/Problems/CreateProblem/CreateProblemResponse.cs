namespace CodeClash.Application.Problems.CreateProblem;
public sealed record CreateProblemResponse(
    Guid Id,
    string Name,
    string Difficulty);
