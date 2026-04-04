namespace CodeClash.Domain.Premitives.Responses;
public class BaseSubmissionResponse
{
    public decimal ExecutionTime { get; set; }

    public string Code { get; set; }

    public DateTime SubmissionDate { get; set; } = DateTime.UtcNow;

    public SubmissionResult SubmissionResult { get; set; } = SubmissionResult.Accepted;
}
