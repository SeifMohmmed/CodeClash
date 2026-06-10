using CodeClash.Application.Abstractions.Cache;
using Microsoft.AspNetCore.Mvc;

namespace CodeClash.API.Controllers.Test;

[Route("tests")]
[ApiController]
public class TestsController(
    ICacheService cacheService) : ControllerBase
{
    [HttpGet("standing1")]
    public async Task<IActionResult> testcache()
    {
        await cacheService.TestCache();
        return Ok();

    }
}
