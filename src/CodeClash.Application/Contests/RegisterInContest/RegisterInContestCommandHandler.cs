using CodeClash.Application.Abstractions.CurrentUser;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Contests;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Contests.RegisterInContest;

internal sealed class RegisterInContestCommandHandler(
    IContestRepository contestRepository,
    ICurrentUserService currentUserService,
    IUserContestRepository userContestRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<RegisterInContestCommand, RegisterInContestResponse>
{
    public async Task<Result<RegisterInContestResponse>> Handle(
        RegisterInContestCommand request,
        CancellationToken cancellationToken)
    {
        var contest = await contestRepository.GetByIdAsync(request.Id);

        if (contest is null)
        {
            return Result.Failure<RegisterInContestResponse>(
                new Error("Contest.NotFound", "No contest found"));
        }

        if (contest.ContestStatus == ContestStatus.Ended)
        {
            return Result.Failure<RegisterInContestResponse>(
                new Error("Contest.Ended", "Contest has already ended"));
        }

        var userId = currentUserService.IdentityId!;

        var isRegisterd = await userContestRepository.IsRegistered(request.Id, userId, cancellationToken);

        if (isRegisterd)
        {
            return Result.Failure<RegisterInContestResponse>(
                new Error("Contest.AlreadyRegistered", "Already registered in this contest"));
        }

        await userContestRepository.AddAsync(
            new UserContest { UserId = userId, ContestId = request.Id },
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(
            new RegisterInContestResponse(request.Id),
            "Registered successfully.");
    }
}
