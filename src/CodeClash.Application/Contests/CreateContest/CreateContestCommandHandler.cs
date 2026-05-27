using CodeClash.Application.Abstractions.CurrentUser;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Mapping;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Contests.CreateContest;
internal sealed class CreateContestCommandHandler(
    IContestRepository contestRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateContestCommand, CreateContestResponse>
{
    public async Task<Result<CreateContestResponse>> Handle(
        CreateContestCommand request,
        CancellationToken cancellationToken)
    {
        var user = await currentUserService.GetUserAsync();
        if (user is null)
        {
            return Result.Failure<CreateContestResponse>(
                new Error("Auth.Unauthorized", "Unauthorized"));
        }

        if (request.StartTime >= request.EndTime)
        {
            return Result.Failure<CreateContestResponse>(
                new Error("Contest.InvalidDates", "Start time must be before end time"));
        }

        var contest = request.ToContest(user.Id);

        contestRepository.Add(contest);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(contest.ToCreateContestResponse(), "Contest Created Successfully");
    }
}
