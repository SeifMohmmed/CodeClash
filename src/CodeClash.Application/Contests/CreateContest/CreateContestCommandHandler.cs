using CodeClash.Application.Abstractions.CurrentUser;
using CodeClash.Application.Abstractions.Job;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Mapping;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Premitives;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace CodeClash.Application.Contests.CreateContest;

internal sealed class CreateContestCommandHandler(
    IContestRepository contestRepository,
    ICurrentUserService currentUserService,
    IBackgroundJobClient backgroundJobClient,
    IUnitOfWork unitOfWork,
    ILogger<CreateContestCommandHandler> logger)
    : ICommandHandler<CreateContestCommand, CreateContestResponse>
{
    public async Task<Result<CreateContestResponse>> Handle(
        CreateContestCommand request,
        CancellationToken cancellationToken)
    {
        var contest = request.ToContest(currentUserService.IdentityId!);

        contestRepository.Add(contest);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Schedule rank-up job to fire when contest ends.
        // If EndDate is already in the past (shouldn't happen given StartTime < EndTime
        // validation, but guards against clock skew/edge cases), fire immediately.
        var delay = contest.EndDate - DateTimeOffset.UtcNow;
        try
        {
            backgroundJobClient.Schedule<IContestRankUpJob>(
                job => job.ExecuteAsync(contest.Id),
                delay > TimeSpan.Zero ? delay : TimeSpan.Zero);
        }

        catch (Exception ex)
        {
            // Contest is already persisted; don't fail the request if scheduling fails.
            // Log for manual follow-up / alerting.
            logger.LogError(
                ex,
                "Failed to schedule rank-up job for contest {ContestId}",
                contest.Id);
        }

        return Result.Success(contest.ToCreateContestResponse(), "Contest Created Successfully");
    }
}
