using CodeClash.Application.Abstractions.Cache;
using CodeClash.Application.Abstractions.CurrentUser;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Mapping;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Premitives;
using CodeClash.Domain.Premitives.Responses;

namespace CodeClash.Application.Contests.GetContest;
internal sealed class GetContestQueryHandler(
    IResponseCacheService cacheService,
    IContestRepository contestRepository,
    ICurrentUserService currentUserService,
    IUserContestRepository userContestRepository)
    : IQueryHandler<GetContestQuery, IReadOnlyList<ContestProblemResponse>>
{
    private const int CacheDurationHours = 2;

    public async Task<Result<IReadOnlyList<ContestProblemResponse>>> Handle(
        GetContestQuery request,
        CancellationToken cancellationToken)
    {
        var user = await currentUserService.GetUserAsync();
        if (user is null)
        {
            return Result.Failure<IReadOnlyList<ContestProblemResponse>>(
                new Error("Auth.Unauthorized", "Unauthorized"));
        }

        var contest = await contestRepository.GetByIdAsync(request.Id);

        if (contest is null)
        {
            return Result.Failure<IReadOnlyList<ContestProblemResponse>>(
                 new Error("Contest.NotFound", "Contest not found"));
        }

        var isRegisterd = await userContestRepository.IsRegistered(request.Id, user.Id);

        if (!isRegisterd)
        {
            return Result.Failure<IReadOnlyList<ContestProblemResponse>>(
            new Error("Contest.NotRegistered", "You are not registered in this contest"));
        }

        if (contest.ContestStatus == ContestStatus.Running)
        {
            return await HandleRunningContestAsync(request.Id);
        }

        return await FetchProblemsAsync(request.Id);
    }

    private async Task<Result<IReadOnlyList<ContestProblemResponse>>> HandleRunningContestAsync(
        Guid contestId)
    {
        string cacheKey = GenerateCacheKey(contestId);

        var cachedData = await cacheService.GetCachedResponseAsync(cacheKey);
        if (cachedData is not null)
        {
            var serialized = Helper.DeserializeCollection<ContestProblemResponse>(cachedData);
            return Result.Success<IReadOnlyList<ContestProblemResponse>>(
                serialized.ToList(), "Contest Problems fetched successfully");
        }

        var problems = await contestRepository.GetContestProblemsByIdAsync(contestId);
        var mapped = problems.Select(p => p.ToContestProblemResponse()).ToList();

        await cacheService.CacheResponseAsync(cacheKey, mapped, TimeSpan.FromHours(CacheDurationHours));

        return Result.Success<IReadOnlyList<ContestProblemResponse>>(
            mapped, "Contest Problems fetched successfully");
    }

    private async Task<Result<IReadOnlyList<ContestProblemResponse>>> FetchProblemsAsync(
        Guid contestId)
    {
        var problems = await contestRepository.GetContestProblemsByIdAsync(contestId);
        var mapped = problems.Select(p => p.ToContestProblemResponse()).ToList();

        return Result.Success<IReadOnlyList<ContestProblemResponse>>(
            mapped, "Contest Problems fetched successfully");
    }

    private static string GenerateCacheKey(Guid contestId) =>
       $"contest-problems:{contestId}";
}
