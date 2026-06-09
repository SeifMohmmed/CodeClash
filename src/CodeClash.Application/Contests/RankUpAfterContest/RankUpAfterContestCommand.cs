using CodeClash.Application.Abstractions.Messaging;

namespace CodeClash.Application.Contests.RankUpAfterContest;

public sealed record RankUpAfterContestCommand(
    Guid ContestId) : ICommand;
