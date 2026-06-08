using CodeClash.Domain.Models.Submits;

namespace CodeClash.Application.Abstractions.Cache;

public interface IResponseCacheService
{
    Task CacheResponseAsync(
        string key,
        object Response,
        TimeSpan timeToLive);

    Task UpdateContestCache(Submit submission);

    Task<string> GetCachedResponseAsync(string key);
}
