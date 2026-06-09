using CodeClash.Domain.Premitives;

namespace CodeClash.Domain.Abstractions;

public class StandingDto
{
    public string UserId { get; set; }

    public string UserName { get; set; }

    public string? UserImage { get; set; }

    public decimal Rank { get; set; }

    public List<Dictionary<int, UserProblemSubmission>> UserProblemSubmissions { get; set; }
}

public class UserProblemSubmission
{
    public Guid SubmissionId { get; set; }
    public DateTime SubmissionDate { get; set; }

    public Language Language { get; set; }
    public int FailCount { get; set; }
}
