using CodeClash.Application.Abstractions.CurrentUser;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Contests;
using CodeClash.Domain.Models.Problems;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Contests.AddContestProblems;

internal sealed class AddContestProblemCommandHandler(
    IContestRepository contestRepository,
    IProblemRepository problemRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork) : ICommandHandler<AddContestProblemCommand, AddContestProblemResponse>
{
    private const int MaxProblemsPerContest = 5;
    public async Task<Result<AddContestProblemResponse>> Handle(
        AddContestProblemCommand request,
        CancellationToken cancellationToken)
    {
        var contest = await contestRepository.GetByIdAsync(request.ContestId);
        if (contest is null)
        {
            return Result.Failure<AddContestProblemResponse>(ContestErrors.NotFound);
        }

        var userId = currentUserService.IdentityId!;

        if (contest.SetterId != userId)
        {
            return Result.Failure<AddContestProblemResponse>(
                new Error("Contest.Forbidden", "Only the contest creator can add problems"));
        }

        if (contest.ContestStatus != ContestStatus.Upcoming)
        {
            return Result.Failure<AddContestProblemResponse>(
                new Error("Contest.Locked", "Problems can only be added before the contest starts"));
        }

        var currentProblemCount = await contestRepository
            .GetProblemCountAsync(request.ContestId);

        if (currentProblemCount >= MaxProblemsPerContest)
        {
            return Result.Failure<AddContestProblemResponse>(
                new Error("Contest.ProblemLimitReached",
                $"A contest cannot have more than {MaxProblemsPerContest} problems"));
        }

        var problem = await problemRepository
            .GetByIdAsync(request.ProblemId);

        if (problem is null)
        {
            return Result.Failure<AddContestProblemResponse>(ProblemErrors.NotFound);
        }

        var alreadyExists = await contestRepository
            .HasProblemAsync(request.ContestId, request.ProblemId);

        if (alreadyExists)
        {
            return Result.Failure<AddContestProblemResponse>(
                new Error("Contest.DuplicateProblem", "This problem is already in the contest"));
        }

        await contestRepository.AddProblemAsync(request.ContestId, request.ProblemId);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(
            new AddContestProblemResponse(problem.Id, contest.Id, problem.Name),
            "Problem added to contest successfully.");
    }
}
