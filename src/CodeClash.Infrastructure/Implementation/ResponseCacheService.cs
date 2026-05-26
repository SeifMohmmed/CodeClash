using System.Text.Json;
using CodeClash.Application.Abstractions.Cache;
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
    public async Task<IEnumerable<T>> GetCachedResponseAsync<T>(
        string key) where T : class
    {
        // Retrieve value from Redis
        var value = await _database.StringGetAsync(key);

        // Return null if cache entry does not exist
        if (!value.HasValue || value.IsNullOrEmpty)
        {
            return null;
        }

        // Cast RedisValue to string explicitly to avoid null warning
        var json = (string?)value;

        // Ensure cached JSON is not empty
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        // Deserialize JSON back into collection of T
        return JsonSerializer.Deserialize<IEnumerable<T>>(json, _serializerOptions);
    }
}
