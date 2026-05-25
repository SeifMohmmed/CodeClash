namespace CodeClash.Domain.Premitives.Responses;
public sealed class TimeLimitExceedResponse : BaseSubmissionResponse
{
    public string Input { get; set; }

    public string ExpectedOutput { get; set; }
}
