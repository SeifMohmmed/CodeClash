using CodeClash.Domain.Premitives;

namespace CodeClash.Application.DTO;

public sealed class SubmissionDto
{
    public string UserId { get; set; }
    public string Code { get; set; }
    public DateTime SubmissionDate { get; set; }
    public Language Language { get; set; }
}
