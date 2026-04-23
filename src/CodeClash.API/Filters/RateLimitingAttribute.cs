using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CodeClash.API.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public sealed class RateLimitingAttribute : Attribute, IAsyncActionFilter
{
    private static readonly ConcurrentDictionary<string, RateLimitInfo> RateLimitStore = new();
    private readonly int _maxRequestsPerMinute;
    private static readonly TimeSpan TimeWindow = TimeSpan.FromMinutes(1);
    public RateLimitingAttribute(
        int maxRequestPerMin)
    {
        _maxRequestsPerMinute = maxRequestPerMin;
    }
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var ipAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString();

        if (ipAddress is null)
        {
            context.Result = new BadRequestObjectResult("Unable to determine client IP.");
            return;
        }

        if (IsRateLimited(ipAddress))
        {
            context.Result = new ObjectResult(
                $"You can't run code more than {_maxRequestsPerMinute} times per minute.")
            {
                StatusCode = 429
            };
            return;
        }


        await next();
    }


    private bool IsRateLimited(string ipAddress)
    {
        var currentTime = DateTime.UtcNow;

        var rateLimitInfo = RateLimitStore.GetOrAdd(ipAddress, new RateLimitInfo());

        rateLimitInfo.RequestTimes = rateLimitInfo.RequestTimes
            .Where(t => currentTime - t <= TimeWindow)
            .ToList();

        if (rateLimitInfo.RequestTimes.Count >= _maxRequestsPerMinute)
        {
            return true;  // Rate limit exceeded
        }

        rateLimitInfo.RequestTimes.Add(currentTime);
        return false;
    }

    private sealed class RateLimitInfo
    {
        public List<DateTime> RequestTimes { get; set; } = new List<DateTime>();
    }
}
