namespace CodeClash.Domain.Premitives.Responses;
public sealed class RunTimeErrorResponse : BaseSubmissionResponse
{
    public Guid TestCaseNumber { get; set; }
    public string Message { get; set; }
}
