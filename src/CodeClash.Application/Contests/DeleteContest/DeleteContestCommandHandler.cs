using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.DTO;
using CodeClash.Application.Mapping;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Contests;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Contests.DeleteContest;

internal sealed class DeleteContestCommandHandler(
    IContestRepository contestRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteContestCommand, ContestResponseDto>
{
    public async Task<Result<ContestResponseDto>> Handle(
        DeleteContestCommand request,
        CancellationToken cancellationToken)
    {
        var contest = await contestRepository.GetByIdAsync(request.Id);

        if (contest is null)
        {
            return Result.Failure<ContestResponseDto>(ContestErrors.NotFound);
        }

        contestRepository.Delete(contest);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var response = contest.ToResponseDto();

        return Result.Success(response, "Contest deleted successfully.");

    }
}
