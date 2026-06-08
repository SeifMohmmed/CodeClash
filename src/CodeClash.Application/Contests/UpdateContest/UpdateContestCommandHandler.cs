using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.DTO;
using CodeClash.Application.Mapping;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Contests.UpdateContest;

internal sealed class UpdateContestCommandHandler(
    IContestRepository contestRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateContestCommand, ContestResponseDto>
{
    public async Task<Result<ContestResponseDto>> Handle(
        UpdateContestCommand request,
        CancellationToken cancellationToken)
    {
        var contest = await contestRepository.GetByIdAsync(request.Id);

        if (contest is null)
        {
            return Result.Failure<ContestResponseDto>(
                new Error("Contest.Not.Found",
                "Contest not found."));
        }

        // Apply changes to the domain entity
        contest.Update(
            request.Name,
            request.StartDate,
            request.EndDate);

        contestRepository.Update(contest);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var response = contest.ToResponseDto();

        return Result.Success(response, "Contest updated successfully.");

    }
}
