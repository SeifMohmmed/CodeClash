using CodeClash.Application.Abstractions.Job;
using CodeClash.Application.Contests.RankUpAfterContest;
using MediatR;

namespace CodeClash.Infrastructure.Implementation;

internal sealed class ContestRankUpJob(
    ISender sender) : IContestRankUpJob
{
    public async Task ExecuteAsync(Guid contestId)
    {
        await sender.Send(
            new RankUpAfterContestCommand(contestId));
    }
}
