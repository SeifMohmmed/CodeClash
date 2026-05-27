using CodeClash.Application.Abstractions.CurrentUser;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Contests;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Contests.RegisterInContest;
internal sealed class RegisterInContestCommandHandler(
    IContestRepository contestRepository,
    ICurrentUserService currentUserService,
    IUserContestRepository userContestRepository)
    : ICommandHandler<RegisterInContestCommand, RegisterInContestResponse>
{
    public async Task<Result<RegisterInContestResponse>> Handle(
        RegisterInContestCommand request,
        CancellationToken cancellationToken)
    {
        var user = await currentUserService.GetUserAsync();
        if (user is null)
        {
            return Result.Failure<RegisterInContestResponse>(
                new Error("Auth.Unauthorized", "Unauthorized"));
        }

        var contest = await contestRepository.GetByIdAsync(request.Id);

        if (contest is null)
        {
            return Result.Failure<RegisterInContestResponse>(
                new Error("Contest.NotFound", "No contest found"));
        }

        var isRegisterd = await userContestRepository.IsRegistered(request.Id, user.Id);

        if (isRegisterd)
        {
            return Result.Failure<RegisterInContestResponse>(
                new Error("Contest.AlreadyRegistered", "Already registered in this contest"));
        }

        var registration = new UserContest
        {
            UserId = user.Id,
            ContestId = request.Id,
        };

        await userContestRepository.RegisterInContest(registration);

        return Result.Success(new RegisterInContestResponse(request.Id));
    }
}
