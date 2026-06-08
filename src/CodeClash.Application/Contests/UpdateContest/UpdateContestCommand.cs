using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.DTO;

namespace CodeClash.Application.Contests.UpdateContest;

public sealed record UpdateContestCommand(
    Guid Id,
    string Name,
    DateTime StartDate,
    DateTime EndDate) : ICommand<ContestResponseDto>;
