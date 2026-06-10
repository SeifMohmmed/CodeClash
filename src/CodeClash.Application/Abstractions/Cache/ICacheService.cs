using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Submits;
using CodeClash.Domain.Premitives;
using CodeClash.Domain.Requests;

namespace CodeClash.Application.Abstractions.Cache;

public interface ICacheService
{
    Task CacheResponseAsync(
        string key,
        object Response,
        TimeSpan timeToLive);

    Task UpdateContestCache(
        Submit submission);

    Task<string> GetCachedResponseAsync(
        string key);

    void UpdateStanding(
        ContestPoints points,
        UserToCache user,
        Guid contestId);

    Task<IReadOnlyList<StandingDto>> GetContestStanding(
        Guid contestId,
        int start,
        int stop);

    Task TestCache();
}
