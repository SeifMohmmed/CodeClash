using System.Text.Json;
using CodeClash.Application.Abstractions.Cache;
using CodeClash.Domain.Models.Submits;
using CodeClash.Domain.Premitives;
using StackExchange.Redis;

namespace CodeClash.Infrastructure.Implementation;
/// <summary>
/// Service responsible for caching and retrieving API responses using Redis.
/// </summary>
internal sealed class ResponseCacheService : IResponseCacheService
{
    // Cache and reuse JsonSerializerOptions instead of creating per call
    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    // Redis database instance used for cache operations.
    private readonly IDatabase _database;

    public ResponseCacheService(IConnectionMultiplexer multiplexer)
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

    public async Task UpdateContestCache(Submit submission)
    {
        string userKey = $"leaderboard:user:{submission.UserId}";
        string globalKey = "leaderboard:global";

        string problemField = $"problem:{submission.ProblemId}";
        string submissionData = $"{submission.Id},{submission.SubmissionDate:O},{(int)submission.Result}";

        var db = _database;

        // Check if the problem was already solved
        bool alreadyAccepted = false;
        var existingSubmission = await db.HashGetAsync(userKey, problemField);
        if (existingSubmission.HasValue)
        {
            var submissionParts = existingSubmission.ToString().Split(',');
            if (submissionParts.Length > 2 && Enum.TryParse(submissionParts[2], out SubmissionResult result))
            {
                alreadyAccepted = result == SubmissionResult.Accepted;
            }
        }

        // Start Redis transaction
        var tran = db.CreateTransaction();

        tran.AddCondition(Condition.HashEqual(userKey, problemField, existingSubmission));

        _ = tran.HashSetAsync(userKey, problemField, submissionData);


        // Update Global Score only if it's the first accepted submission for this problem
        if (submission.Result == SubmissionResult.Accepted && !alreadyAccepted)
        {
            _ = tran.SortedSetIncrementAsync(globalKey, submission.UserId, 1);
        }

        // Execute transaction
        await tran.ExecuteAsync();

    }
}
