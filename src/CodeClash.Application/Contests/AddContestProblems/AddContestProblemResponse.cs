namespace CodeClash.Application.Contests.AddContestProblems;
public record AddContestProblemResponse(
    Guid ContestId,
    Guid ProblemId,
    string ProblemName);
