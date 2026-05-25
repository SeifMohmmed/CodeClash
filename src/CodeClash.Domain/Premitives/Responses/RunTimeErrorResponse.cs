namespace CodeClash.Domain.Premitives.Responses;
public sealed class RunTimeErrorResponse : BaseSubmissionResponse
{
    public string Message { get; set; }

    public string Input { get; set; }

    public string ExpectedOutput { get; set; }

}
