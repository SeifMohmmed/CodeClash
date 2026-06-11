using CodeClash.Domain.Premitives;

namespace CodeClash.Domain.Requests;

public sealed class SubmissionToCache
{
    public string UserId { get; set; }
    public Guid ProblemId { get; set; }
    public SubmissionResult Result { get; set; }
    public Language Language { get; set; }
    public DateTime Date { get; set; }
}
