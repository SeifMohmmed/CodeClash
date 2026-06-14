using CodeClash.API.Filters;
using CodeClash.Application.RunCode;
using CodeClash.Application.SolveProblem.SubmitSolutions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeClash.API.Controllers.Solutions;

[Route("solutions")]
[ApiController]
public class SolutionsController(
    ISender sender) : ControllerBase
{
    [Authorize]
    [HttpPost]
    [RateLimitingAttribute(5)]
    public async Task<IActionResult> Solve(
        [FromForm] SubmitSolutionCommand command,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(command, cancellationToken);

        return response.IsFailure
            ? BadRequest(response)
            : Ok(response);
    }

    [Authorize]
    [HttpPost("run")]
    [RateLimitingAttribute(5)]
    public async Task<IActionResult> RunCode(
    [FromForm] RunCodeCommand command,
    CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result);
    }
}
