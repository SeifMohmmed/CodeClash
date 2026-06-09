namespace CodeClash.Application.Abstractions.Job;

public interface IContestRankUpJob
{
    Task ExecuteAsync(Guid contestId);
}
