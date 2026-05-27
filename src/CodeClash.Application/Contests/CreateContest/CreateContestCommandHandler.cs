using System.Security.Claims;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Mapping;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Premitives;
using Microsoft.AspNetCore.Http;

namespace CodeClash.Application.Contests.CreateContest;
internal sealed class CreateContestCommandHandler(
    IContestRepository contestRepository,
    IUnitOfWork unitOfWork,
    IHttpContextAccessor contextAccessor)
    : ICommandHandler<CreateContestCommand, CreateContestResponse>
{
    public async Task<Result<CreateContestResponse>> Handle(
        CreateContestCommand request,
        CancellationToken cancellationToken)
    {
        var userId = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
        {
            return Result.Failure<CreateContestResponse>(new Error("Auth.Error", "Unauthorized"));
        }

        if (request.StartTime >= request.EndTime)
        {
            return Result.Failure<CreateContestResponse>(
                new Error("Contest.InvalidDates", "Start time must be before end time"));
        }

        var contest = request.ToContest(userId);

        contestRepository.Add(contest);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(contest.ToCreateContestResponse(), "Contest Created Successfully");
    }
}
