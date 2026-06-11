namespace CodeClash.Domain.Abstractions;

public class StandingDto
{
    public string UserId { get; set; }

    public string UserName { get; set; }

    public string? UserImage { get; set; }

    public decimal Rank { get; set; }

    public List<UserProblemSubmission> UserProblemSubmissions { get; set; }
}

public class UserProblemSubmission : ProblemSubmissionsCount
{
    public Guid ProblemId { get; set; }

    public DateTime? EarliestSuccessDate { get; set; }

}

public class ProblemSubmissionsCount
{
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
}
