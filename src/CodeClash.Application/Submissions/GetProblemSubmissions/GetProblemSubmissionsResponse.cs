using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Submissions.GetProblemSubmissions;
public sealed class GetProblemSubmissionsResponse
{
    public decimal SubmitTime { get; set; }
    public decimal SubmitMemory { get; set; }
    public SubmissionResult Result { get; set; }
    public string Code { get; set; }
    public DateTime SubmissionDate { get; set; }
    public Language Language { get; set; }
}
