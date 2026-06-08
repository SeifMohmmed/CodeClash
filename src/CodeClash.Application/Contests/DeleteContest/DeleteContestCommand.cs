using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.DTO;

namespace CodeClash.Application.Contests.DeleteContest;

public sealed record DeleteContestCommand(
    Guid Id) : ICommand<ContestResponseDto>;
