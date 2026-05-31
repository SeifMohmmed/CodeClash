using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Contests.GetAllContests;
public record GetAllContestsResponse(
    Guid Id,
    string Name,
    DateTime StartDate,
    DateTime EndDate,
    TimeSpan Duration,
    ContestStatus ContestStatus,
    int ParticipantsCount,
    int ProblemsCount);
