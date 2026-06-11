using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Premitives;
using CodeClash.Domain.Requests;

namespace CodeClash.Application.Abstractions.Cache;

public interface ICacheService
{
    Task CacheResponseAsync(
        string key,
        object Response,
        TimeSpan timeToLive);

    Task<string?> GetCachedResponseAsync(
        string key);

    Task CacheContestStandingAsync(
        ContestPoints points,
        UserToCache user,
        Guid contestId);

    Task<IReadOnlyList<StandingDto>> GetContestStanding(
        Guid contestId,
        int start,
        int stop);

    Task CacheUserSubmissionAsync(
        SubmissionToCache submission,
        string userId,
        Guid contestId);

    Task<bool> IsUserSolvedTheProblemAsync(
        string userId,
        Guid contestId,
        Guid problemId);
}
