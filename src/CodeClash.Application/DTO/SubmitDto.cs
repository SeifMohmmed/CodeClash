using CodeClash.Domain.Premitives;

namespace CodeClash.Application.DTO;

public sealed class SubmitDto
{
    public string UserId { get; set; }
    public Guid ProblemId { get; set; }
    public Guid? ContestId { get; set; }
    public string Code { get; set; }
    public Language Language { get; set; }
    public DateTime SubmissionDate { get; set; }
    public SubmissionResult Result { get; set; }
}
