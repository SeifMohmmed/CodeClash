using CodeClash.Application.Abstractions.Messaging;

namespace CodeClash.Application.Contests.RegisterInContest;
public record RegisterInContestCommand(
    Guid Id) : ICommand<RegisterInContestResponse>;
