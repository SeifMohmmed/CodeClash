using CodeClash.Application.Contests.CreateContest;
using CodeClash.Application.Contests.GetAllContests;
using CodeClash.Domain.Models.Contests;

namespace CodeClash.Application.Mapping;
public static class ContestMappings
{
    public static Contest ToContest(this CreateContestCommand command, string userId)
        => new()
        {
            Name = command.Name,
            SetterId = userId,
            StartDate = DateTime.SpecifyKind(command.StartTime, DateTimeKind.Utc),
            EndDate = DateTime.SpecifyKind(command.EndTime, DateTimeKind.Utc),
        };

    public static CreateContestResponse ToCreateContestResponse(this Contest contest)
    => new(
        contest.Id,
        contest.Name);

    public static GetAllContestsResponse ToGetAllContestsResponse(this Contest contest)
    => new(
        contest.Id,
        contest.Name,
        contest.StartDate,
        contest.EndDate,
        contest.Duration,
        contest.ContestStatus,
        contest.Registrations?.Count ?? 0,
        contest.Problems?.Count ?? 0
    );
}
