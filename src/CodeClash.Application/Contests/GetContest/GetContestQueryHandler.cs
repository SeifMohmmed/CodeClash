using System.Globalization;
using System.Text;
using CodeClash.Application.Abstractions.Cache;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Mapping;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Premitives;
using CodeClash.Domain.Premitives.Responses;
using Microsoft.AspNetCore.Http;

namespace CodeClash.Application.Contests.GetContest;
internal sealed class GetContestQueryHandler(
    IResponseCacheService cacheService,
    IHttpContextAccessor httpContext,
    IContestRepository contestRepository)
    : IQueryHandler<GetContestQuery, IReadOnlyList<ContestProblemResponse>>
{
    public async Task<Result<IReadOnlyList<ContestProblemResponse>>> Handle(
        GetContestQuery request,
        CancellationToken cancellationToken)
    {
        var contest = await contestRepository.GetByIdAsync(request.Id);

        if (contest is null)
        {
            return Result.Failure<IReadOnlyList<ContestProblemResponse>>(new Error("Contest.Not.Found", "Not Found!"));
        }

        if (contest.ContestStatus == ContestStatus.Upcoming)
        {
            return Result.Failure<IReadOnlyList<ContestProblemResponse>>(new Error("Contest.Not.Started", "Not Started yet!"));
        }

        if (contest.ContestStatus == ContestStatus.Running)
        {
            string cacheKey = GenerateCacheKeyFromRequest();

            // check cache
            var cachedData = await cacheService.GetCachedResponseAsync(cacheKey);

            // cache hit → return cached data
            if (cachedData is not null)
            {
                var serializedData = Helper.DeserializeCollection<ContestProblemResponse>(cachedData);

                return Result.Success<IReadOnlyList<ContestProblemResponse>>(
                    serializedData.ToList(),
                    "Contest Problems fetched successfully");
            }

            // cache miss → get from db, cache it, return it
            var problems =
                await contestRepository.GetContestProblemsByIdAsync(request.Id);

            var mappedResponse = problems
                .Select(p => p.ToContestProblemResponse())
                .ToList();

            await cacheService.CacheResponseAsync(cacheKey, mappedResponse, TimeSpan.FromHours(2));

            return Result.Success<IReadOnlyList<ContestProblemResponse>>(
                mappedResponse, "Contest Problems fetched successfully");
        }

        // past → get from db directly, no caching
        var dbProblems = await contestRepository.GetContestProblemsByIdAsync(request.Id);
        var response = dbProblems
            .Select(p => p.ToContestProblemResponse())
            .ToList();

        return Result.Success<IReadOnlyList<ContestProblemResponse>>(
            response, "Contest Problems Feteched Successfully");
    }

    private string GenerateCacheKeyFromRequest()
    {
        // key : unique for each request so generate it from request
        // generate it from URL Path + Query String 
        var request = httpContext.HttpContext.Request;
        var keyBuilder = new StringBuilder();

        keyBuilder.Append(request.Path);

        // Ordered by key to handle cases when the order of query string
        // parameters changes but the values remain the same.
        // use InvariantCulture to avoid locale-dependent formatting warning

        foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
        {
            keyBuilder.Append(CultureInfo.InvariantCulture, $"|{key}-{value}");
        }

        return keyBuilder.ToString();
    }
}
