using CodeClash.Application.Abstractions.Job;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.DTO;
using CodeClash.Application.Mapping;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Contests;
using CodeClash.Domain.Premitives;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace CodeClash.Application.Contests.UpdateContest;

internal sealed class UpdateContestCommandHandler(
    IContestRepository contestRepository,
    IUnitOfWork unitOfWork,
    IBackgroundJobClient backgroundJobClient,
    ILogger<UpdateContestCommandHandler> logger)
    : ICommandHandler<UpdateContestCommand, ContestResponseDto>
{
    public async Task<Result<ContestResponseDto>> Handle(
        UpdateContestCommand request,
        CancellationToken cancellationToken)
    {
        var contest = await contestRepository.GetByIdAsync(request.Id);

        if (contest is null)
        {
            return Result.Failure<ContestResponseDto>(ContestErrors.NotFound);
        }

        if (contest.ContestStatus == ContestStatus.Ended)
        {
            return Result.Failure<ContestResponseDto>(
                new Error("Contest.Ended", "Cannot update a contest that has already ended."));
        }

        var endDateChanged = contest.EndDate != request.EndDate;

        // Apply changes to the domain entity
        contest.Update(
            request.Name,
            request.StartDate,
            request.EndDate);

        contestRepository.Update(contest);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (endDateChanged)
        {
            var delay = DateTime.SpecifyKind(contest.EndDate, DateTimeKind.Utc) - DateTime.UtcNow;
            try
            {
                backgroundJobClient.Schedule<IContestRankUpJob>(
                    job => job.ExecuteAsync(contest.Id),
                    delay > TimeSpan.Zero ? delay : TimeSpan.Zero);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Failed to reschedule rank-up job for contest {ContestId}",
                    contest.Id);
            }
        }

        var response = contest.ToResponseDto();

        return Result.Success(response, "Contest updated successfully.");

    }
}
