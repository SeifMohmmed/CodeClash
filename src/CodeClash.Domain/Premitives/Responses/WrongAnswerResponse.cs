namespace CodeClash.Domain.Premitives.Responses;
public sealed class WrongAnswerResponse : BaseSubmissionResponse
{
    public Guid TestCaseNumber { get; set; }
    public string ExpectedOutput { get; set; }
    public string ActualOutput { get; set; }
}
