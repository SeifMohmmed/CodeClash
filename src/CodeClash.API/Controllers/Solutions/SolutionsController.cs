using CodeClash.API.Filters;
using CodeClash.Application.RunCode;
using CodeClash.Application.SolveProblem.SubmitSolutions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeClash.API.Controllers.Solutions;
/// <summary>
/// Solution submission and code execution endpoints.
/// </summary>
[Route("solutions")]
[ApiController]
public class SolutionsController(
    ISender sender) : ControllerBase
{
    /// <summary>Submit a solution.</summary>
    [Authorize]
    [HttpPost]
    [RateLimitingAttribute(5)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Solve(
        [FromForm] SubmitSolutionCommand command,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(command, cancellationToken);

        return response.IsFailure
            ? BadRequest(response)
            : Ok(response);
    }

    /// <summary>Run code without submitting.</summary>
    [Authorize]
    [HttpPost("run")]
    [RateLimitingAttribute(5)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
