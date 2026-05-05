namespace CodeClash.Domain.Premitives.Responses;
public sealed class WrongAnswerResponse : BaseSubmissionResponse
{
    public string ExpectedOutput { get; set; }
    public string ActualOutput { get; set; }
}
