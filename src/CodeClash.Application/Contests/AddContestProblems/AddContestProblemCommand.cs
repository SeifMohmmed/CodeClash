using CodeClash.Application.Abstractions.Messaging;

namespace CodeClash.Application.Contests.AddContestProblems;
public sealed record AddContestProblemCommand(
    Guid ContestId,
    Guid ProblemId) : ICommand<AddContestProblemResponse>;

