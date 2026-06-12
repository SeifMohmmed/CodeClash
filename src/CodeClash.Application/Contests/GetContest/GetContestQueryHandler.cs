using CodeClash.Application.Abstractions.Cache;
using CodeClash.Application.Abstractions.CurrentUser;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Mapping;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Contests;
using CodeClash.Domain.Premitives;
using CodeClash.Domain.Premitives.Responses;

namespace CodeClash.Application.Contests.GetContest;

internal sealed class GetContestQueryHandler(
    ICacheService cacheService,
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
        var contest =
            await contestRepository.GetByIdAsync(request.Id);

        if (contest is null)
        {
            return Result.Failure<IReadOnlyList<ContestProblemResponse>>(ContestErrors.NotFound);
        }

        var userId = currentUserService.IdentityId!;

        var isRegisterd = await userContestRepository
            .IsRegistered(request.Id, userId, cancellationToken);

        if (!isRegisterd)
        {
            return Result.Failure<IReadOnlyList<ContestProblemResponse>>(
            new Error("Contest.NotRegistered", "You are not registered in this contest"));
        }

        // Only cache during running contests — problems may change before start
        // and are irrelevant to cache after end.
        if (contest.ContestStatus == ContestStatus.Running)
        {
            string cacheKey = Helper.GenerateContestProblemsKey(request.Id);

            var cachedData = await cacheService.GetCachedResponseAsync(cacheKey);

            if (cachedData is not null)
            {
                // Cache HIT
                var cached = Helper.DeserializeCollection<ContestProblemResponse>(cachedData);
                return Result.Success<IReadOnlyList<ContestProblemResponse>>(cached.ToList());
            }

            // Cache MISS 
            var problems = await contestRepository
                .GetContestProblemsByIdAsync(request.Id);

            var mapped = problems
                .Select(p => p.ToContestProblemResponse())
                .ToList();

            await cacheService.CacheResponseAsync(
                cacheKey, mapped, TimeSpan.FromHours(CacheDurationHours));

            return Result.Success<IReadOnlyList<ContestProblemResponse>>(mapped);
        }

        var allProblems = await contestRepository
            .GetContestProblemsByIdAsync(request.Id);

        return Result.Success<IReadOnlyList<ContestProblemResponse>>(
            allProblems.Select(p => p.ToContestProblemResponse()).ToList());

    }
}
