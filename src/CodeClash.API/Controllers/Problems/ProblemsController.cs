using CodeClash.Application.Problems.CreateProblem;
using CodeClash.Application.Problems.GetProblemTestCases;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CodeClash.API.Controllers.Problems;
[Route("problems")]
[ApiController]
public class ProblemsController(
    ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateProblem(
       [FromBody] CreateProblemCommand command)
    {
        var response = await sender.Send(command);

        if (response.IsFailure)
        {
            return BadRequest(response.Error);
        }

        return CreatedAtAction(nameof(GetTestCases),
            new { problemId = response.Value },
            response.Value);
    }

    [HttpGet("{problemId}/test-cases")]
    public async Task<IActionResult> GetTestCases(Guid problemId)
    {
        var query = new GetProblemTestCaseQuery(problemId);

        var response = await sender.Send(query);

        return response.IsSuccess ? Ok(response.Value) : NotFound();
    }
}
