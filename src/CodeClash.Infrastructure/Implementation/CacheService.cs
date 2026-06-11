using System.Text.Json;
using CodeClash.Application.Abstractions.Cache;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Submits;
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
    /// Stores a user submission inside Redis list.
    /// Used for tracking submission history.
    /// </summary>
    public void CacheUserSubmission(
        SubmissionToCache submission,
        Guid contestId)
    {
        // Generate Redis key
        string key =
            Helper.GenerateUserSubmissionKey(
                submission.UserId,
                contestId);

        // Serialize submission
        string serializedSubmission =
            JsonSerializer.Serialize(submission);

        // Append submission to list
        _database.ListRightPush(
            key,
            serializedSubmission);

        // Keep expiry in sync with the contest standing key
        SetKeyExpiration(key, 2);
    }

    /// <summary>
    /// Retrieves and deserializes a cached response from Redis.
    /// </summary>
    public async Task<string> GetCachedResponseAsync(string key)
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
    public Task<IReadOnlyList<StandingDto>> GetContestStanding(
        Guid contestId,
        int start,
        int stop)
    {
        string key =
            Helper.GenerateContestKey(contestId);

        var leaderboard =
            _database.SortedSetRangeByRankWithScores(
                key,
                start,
                stop,
                StackExchange.Redis.Order.Descending);

        var result =
            leaderboard
            .Select((entry, i) =>
            {
                var user = Helper.ConvertRedisMemberToUser(entry.Element.ToString());

                return new StandingDto
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    UserImage = user.UserImage,

                    // Convert zero-based index to rank
                    Rank = start + i + 1,
                    UserProblemSubmissions = GetUserSubmissionsList(user.UserId, contestId)
                };
            })
            .ToList();

        return Task.FromResult<IReadOnlyList<StandingDto>>(result);
    }

    /// <summary>
    /// Demonstrates Redis SortedSet operations.
    /// Intended only for testing.
    /// </summary>
    public async Task TestCache()
    {
        // Connect to Redis

#pragma warning disable CA1303
        string key = "leaderboard";

        // ZADD → Insert members
        _database.SortedSetAdd(key, "Alice", 1500);
        _database.SortedSetAdd(key, "Bob", 1800);
        _database.SortedSetAdd(key, "Charlie", 1700);

        // Get sorted elements (ZRANGE with scores)
        Console.WriteLine("Leaderboard:");
        foreach (var entry in _database.SortedSetRangeByRankWithScores(key, order: StackExchange.Redis.Order.Descending))
        {
            Console.WriteLine($"{entry.Element}: {entry.Score}");
        }

        // Get the rank of a specific player (ZRANK)
        long? rank = _database.SortedSetRank(key, "Alice", StackExchange.Redis.Order.Descending);
        Console.WriteLine($"Alice's Rank: {rank + 1}");

        // Increment score (ZINCRBY)
        _database.SortedSetIncrement(key, "Alice", 300);
        Console.WriteLine("Alice's new score: " + _database.SortedSetScore(key, "Alice"));

        // Get top 2 players (ZRANGE with limit)
        Console.WriteLine("Top 2 Players:");
        var topPlayers = _database.SortedSetRangeByRankWithScores(key, 0, 1, StackExchange.Redis.Order.Descending);
        foreach (var player in topPlayers)
        {
            Console.WriteLine($"{player.Element}: {player.Score}");
        }
    }

    /// <summary>
    /// Updates global contest cache after submission.
    /// Tracks first accepted submissions only.
    /// </summary>
    public async Task UpdateContestCache(
        Submit submission)
    {
        string userKey =
            $"leaderboard:user:{submission.UserId}";

        string globalKey =
            "leaderboard:global";

        string problemField =
            $"problem:{submission.ProblemId}";

        string submissionData =
           $"{submission.Id}," +
           $"{submission.SubmissionDate:O}," +
           $"{(int)submission.Result}";

        // Check if the problem was already solved
        bool alreadyAccepted = false;

        var existingSubmission = await _database.HashGetAsync(userKey, problemField);

        if (existingSubmission.HasValue)
        {
            var submissionParts = existingSubmission.ToString().Split(',');
            if (submissionParts.Length > 2 && Enum.TryParse(submissionParts[2], out SubmissionResult result))
            {
                alreadyAccepted = result == SubmissionResult.Accepted;
            }
        }

        // Start Redis transaction
        var tran =
            _database.CreateTransaction();

        // Prevent race conditions
        tran.AddCondition(
            Condition.HashEqual(
                userKey,
                problemField,
                existingSubmission));

        // Save latest submission
        _ = tran.HashSetAsync(
            userKey,
            problemField,
            submissionData);

        // Award score only once
        if (submission.Result == SubmissionResult.Accepted
            && !alreadyAccepted)
        {
            _ = tran.SortedSetIncrementAsync(
                globalKey,
                submission.UserId,
                1);
        }

        // Execute transaction
        await tran.ExecuteAsync();

    }

    /// <summary>
    /// Updates contest leaderboard score.
    /// </summary>
    public void CacheContestStanding(
        ContestPoints points,
        UserToCache user,
        Guid contestId)
    {
        string key =
            Helper.GenerateContestKey(contestId);

        string member =
            Helper.ConvertUserToRedisMemeber(user);

        // Increase or add the user with their points
        _database.SortedSetIncrement(
            key,
            member,
            (double)points);

        // Auto-expire leaderboard
        _database.KeyExpire(
            key,
            TimeSpan.FromHours(2));
    }

    public void CacheUserSubmission(
        SubmissionToCache submission,
        string userId,
        Guid contestId)
    {
        string key = Helper.GenerateUserSubmissionKey(userId, contestId);

        string serializedSubmission = JsonSerializer.Serialize(submission);

        _database.ListRightPush(key, serializedSubmission);

        _database.KeyExpire(key, TimeSpan.FromHours(2));
    }

    public bool IsUserSolvedTheProblem(
        string userId,
        Guid contestId,
        Guid problemId)
    {
        string key = Helper.GenerateUserSubmissionKey(userId, contestId);

        return _database
            .ListRange(key, 0, -1)
            .Select(s => JsonSerializer.Deserialize<SubmissionToCache>(s.ToString()))
            .Any(s => s?.ProblemId == problemId && s.Result == SubmissionResult.Accepted);
    }


    #region Private
    private void SetKeyExpiration(string key, double time)
         => _database.KeyExpire(key, TimeSpan.FromHours(time));
    private List<UserProblemSubmission> GetUserSubmissionsList(
        string userId,
        Guid contestId)
    {
        string key = Helper.GenerateUserSubmissionKey(userId, contestId);

        var submissions = _database
            .ListRange(key, 0, -1)
            .Select(s => JsonSerializer.Deserialize<SubmissionToCache>(s.ToString()))
            .Where(s => s is not null)
            .Select(s => s!)
            .ToList();

        // Aggregate counts per problem
        var grouped = submissions
            .GroupBy(s => s.ProblemId)
            .Select(g => new UserProblemSubmission
            {
                ProblemId = g.Key,
                SuccessCount = g.Count(s => s.Result == SubmissionResult.Accepted),
                FailureCount = g.Count(s => s.Result != SubmissionResult.Accepted),
                EarliestSuccessDate = g.First(s => s.Result == SubmissionResult.Accepted)?.Date
                                 ?? g.First().Date,
            })
            .ToList();

        return grouped;
    }
    #endregion
}
