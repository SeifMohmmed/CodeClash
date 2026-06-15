namespace CodeClash.Domain.Requests;

public sealed record CodeSimilarityRequest(
    string Code1,
    string Code2);
