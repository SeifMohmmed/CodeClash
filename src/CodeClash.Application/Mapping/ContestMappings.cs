using CodeClash.Application.Contests.CreateContest;
using CodeClash.Domain.Models.Contests;

namespace CodeClash.Application.Mapping;
public static class ContestMappings
{
    public static Contest ToContest(this CreateContestCommand command, string userId)
        => new()
        {
            Name = command.Name,
            SetterId = userId,
            StartDate = command.StartTime,
            EndDate = command.EndTime,
        };

    public static CreateContestResponse ToCreateContestResponse(this Contest contest)
    => new(
        contest.Id,
        contest.Name);

}
