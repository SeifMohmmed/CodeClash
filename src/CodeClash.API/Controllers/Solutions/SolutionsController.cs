using CodeClash.Application.SolveProblem.SubmitSolutions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CodeClash.API.Controllers.Solutions;
[Route("solutions")]
[ApiController]
public class SolutionsController(
    ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Solve([FromForm] SubmitSolutionCommand command)
    {
        var response = await sender.Send(command);

        if (response.IsFailure)
        {
            return BadRequest(response.Error);
        }

        return Ok(response.Value);
    }
}
