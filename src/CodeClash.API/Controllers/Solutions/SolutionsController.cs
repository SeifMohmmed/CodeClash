using CodeClash.API.Filters;
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
    [HttpPost]
    [RateLimitingAttribute(5)]
    [Authorize]
    public async Task<IActionResult> Solve(
        [FromForm] SubmitSolutionCommand command,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(command, cancellationToken);

        return response.IsFailure
            ? BadRequest(response)
            : Ok(response);
    }
}
