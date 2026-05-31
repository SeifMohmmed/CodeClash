namespace CodeClash.Domain.Requests;

public sealed class CodeSimilarityRequest
{
    public string Code1 { get; set; }
    public string Code2 { get; set; }
}
