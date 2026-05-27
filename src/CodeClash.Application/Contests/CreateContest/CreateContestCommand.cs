using CodeClash.Application.Abstractions.Messaging;

namespace CodeClash.Application.Contests.CreateContest;
public sealed record CreateContestCommand(
    string Name,
    string Description,
    DateTime StartTime,
    DateTime EndTime) : ICommand<CreateContestResponse>;
