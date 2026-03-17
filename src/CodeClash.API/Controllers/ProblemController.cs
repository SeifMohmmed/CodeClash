using CodeClash.Application.Problems.CreateProblem;
using CodeClash.Application.Problems.GetProblemTestCases;
using CodeClash.Application.SolveProblem;
using CodeClash.Application.Submissions.GetProblemSubmissions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CodeClash.API.Controllers;
[Route("problems")]
[ApiController]
public class ProblemController(
    ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateProblemAsync(CreateProblemCommand command)
    {
        var response = await sender.Send(command);

        if (response.IsFailure)
        {
            return BadRequest(response.Error);
        }

        return CreatedAtAction(nameof(GetProblemSubmissions), new { id = response.Value }, response.Value);
    }

    [HttpGet("{problemId}/testcases")]
    public async Task<IActionResult> GetProblemTestCaseAsync(Guid problemId)
    {
        var query = new GetProblemTestCase(problemId);

        var response = await sender.Send(query);

        return response.IsSuccess ? Ok(response.Value) : NotFound();
    }

    [HttpGet("{problemId}/submissions")]
    public async Task<IActionResult> GetAllSubmissions(Guid problemId)
    {
        var query = new GetProblemSubmissions(problemId);

        var response = await sender.Send(query);

        return response.IsSuccess ? Ok(response.Value) : NotFound();
    }

    [HttpPost("solve")]
    public async Task<IActionResult> SolveProblemAsync([FromForm] SubmitSolutionCommand command)
    {
        var response = await sender.Send(command);

        if (response.IsFailure)
        {
            return BadRequest(response.Error);
        }

        return Ok(response);
    }
}
