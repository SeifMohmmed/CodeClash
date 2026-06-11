using System.Text.Json;
using CodeClash.Application.Abstractions.Cache;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Premitives;
using CodeClash.Domain.Requests;
using StackExchange.Redis;

namespace CodeClash.Infrastructure.Implementation;
/// <summary>
/// Provides Redis-based caching operations for:
/// - Generic API response caching
/// - Contest leaderboard management
/// - User submission tracking
/// - Contest standing updates
/// </summary>
internal sealed class CacheService : ICacheService
{
    // Cache and reuse JsonSerializerOptions instead of creating per call
    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    // Redis database instance used for cache operations.
    private readonly IDatabase _database;

    public CacheService(IConnectionMultiplexer multiplexer)
    {
        _database = multiplexer.GetDatabase();
    }

    /// <summary>
    /// Stores a serialized response in Redis cache with a specific expiration time.
    /// </summary>
    public async Task CacheResponseAsync(
        string key,
        object Response,
        TimeSpan timeToLive)
    {
        // Ignore null values
        if (Response is null)
        {
            return;
        }

        // Convert object into JSON string
        var serializedResponse = JsonSerializer.Serialize(Response, _serializerOptions);

        // Store serialized response in Redis
        await _database.StringSetAsync(key, serializedResponse, timeToLive);
    }

    /// <summary>
    /// Retrieves and deserializes a cached response from Redis.
    /// </summary>
    public async Task<string?> GetCachedResponseAsync(string key)
    {
        // Retrieve value from Redis
        var value = await _database.StringGetAsync(key);

        // Return null if cache entry does not exist
        if (!value.HasValue || value.IsNullOrEmpty)
        {
            return null;
        }

        return value;
    }

    /// <summary>
    /// Retrieves contest leaderboard.
    /// </summary>
    public async Task<IReadOnlyList<StandingDto>> GetContestStanding(
        Guid contestId,
        int start,
        int stop)
    {
        string contestKey = Helper.GenerateContestKey(contestId);

        var leaderboard = await _database.SortedSetRangeByRankWithScoresAsync(
            contestKey, start, stop, StackExchange.Redis.Order.Descending);

        if (leaderboard.Length == 0)
        {
            return [];
        }

        var users = leaderboard
            .Select(e => Helper.ConvertRedisMemberToUser(e.Element.ToString()))
            .ToList();

        // Fan out all HashGetAll calls in a single pipeline round-trip
        var batch = _database.CreateBatch();

        var submissionTasks = users
            .Select(u => batch.HashGetAllAsync(
                Helper.GenerateUserSubmissionKey(u.UserId, contestId)))
            .ToList();

        batch.Execute();

        await Task.WhenAll(submissionTasks);

        return users
            .Select((user, i) =>
            {
                var submissions = submissionTasks[i].Result
                    .Select(entry =>
                    {
                        var data = JsonSerializer.Deserialize<ProblemSubmissionsCount>(
                            entry.Value.ToString(), _serializerOptions)!;
                        return new UserProblemSubmission
                        {
                            ProblemId = Guid.Parse(entry.Name.ToString()),
                            SuccessCount = data.SuccessCount,
                            FailureCount = data.FailureCount,
                            EarliestSuccessDate = data.EarliestSuccessDate
                        };
                    })
                    .ToList();

                return new StandingDto
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    UserImage = user.UserImage,
                    Rank = start + i + 1,
                    UserProblemSubmissions = submissions
                };
            })
            .ToList();
    }

    /// <summary>
    /// Atomically increments the user's score in the contest sorted set.
    /// Sets a 2-hour TTL on first write only.
    /// </summary>
    public async Task CacheContestStandingAsync(
        ContestPoints points,
        UserToCache user,
        Guid contestId)
    {
        string key =
            Helper.GenerateContestKey(contestId);

        string member =
            Helper.ConvertUserToRedisMemeber(user);

        // Conditionally set TTL only when the key is brand-new (TTL == -1)
        // to avoid sliding the expiry window on every submission.
        const string luaScript = """
            redis.call('ZINCRBY', KEYS[1], ARGV[1], ARGV[2])
            if redis.call('TTL', KEYS[1]) == -1 then
                redis.call('EXPIRE', KEYS[1], 7200)
            end
            """;

        await _database.ScriptEvaluateAsync(
            luaScript,
            new RedisKey[] { key },
            new RedisValue[] { (int)points, member });
    }

    /// <summary>
    /// Atomically updates per-user problem submission counts in a contest hash.
    /// Tracks success count, failure count, and earliest acceptance date.
    /// Sets a 2-hour TTL on first write only.
    /// </summary>
    public async Task CacheUserSubmissionAsync(
        SubmissionToCache submission,
        string userId,
        Guid contestId)
    {
        string key = Helper.GenerateUserSubmissionKey(userId, contestId);

        string field = submission.ProblemId.ToString();

        // Lua script ensures atomic read-modify-write — no TOCTOU window
        const string luaScript = """
        local existing = redis.call('HGET', KEYS[1], KEYS[2])
        local isNew = 0
        local data

        if existing then
            data = cjson.decode(existing)
        else
            data = { SuccessCount = 0, FailureCount = 0, EarliestSuccessDate = false }
            isNew = 1
        end

        if ARGV[1] == 'Accepted' then
            data.SuccessCount = data.SuccessCount + 1
            if not data.EarliestSuccessDate or ARGV[2] < data.EarliestSuccessDate then
                data.EarliestSuccessDate = ARGV[2]
            end
        else
            data.FailureCount = data.FailureCount + 1
        end

        redis.call('HSET', KEYS[1], KEYS[2], cjson.encode(data))

        -- Set TTL only on first insertion to avoid sliding the window on every submission
        if isNew == 1 then
            redis.call('EXPIRE', KEYS[1], 7200)
        end

        return isNew
        """;

        var keys = new RedisKey[] { key, field };
        var args = new RedisValue[]
        {
        submission.Result == SubmissionResult.Accepted ? "Accepted" : "Failed",
        submission.Date.ToString("O") // ISO 8601 for lexicographic date comparison in Lua
        };

        await _database.ScriptEvaluateAsync(luaScript, keys, args);
    }

    /// <summary>
    /// Returns true if the user has at least one accepted submission
    /// for the given problem in the given contest.
    /// </summary>
    public async Task<bool> IsUserSolvedTheProblemAsync(
        string userId,
        Guid contestId,
        Guid problemId)
    {
        string key = Helper.GenerateUserSubmissionKey(userId, contestId);
        string field = problemId.ToString();

        var value = await _database.HashGetAsync(key, field);
        if (value.IsNullOrEmpty)
        {
            return false;
        }

        var data = JsonSerializer.Deserialize<ProblemSubmissionsCount>(
            value.ToString(),
            _serializerOptions);

        return data?.SuccessCount > 0;
    }
}
