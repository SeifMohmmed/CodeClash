using CodeClash.Application.Problems.CreateProblem;
using CodeClash.Application.Problems.GetPrblemTestcases;
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
       [FromBody] CreateProblemRequest request,
       CancellationToken cancellationToken)
    {
        var command = new CreateProblemCommand(
            request.ContestId,
            request.Name,
            request.Description,
            request.Difficulty,
            request.MemoryLimit,
            request.RunTimeLimit,
            request.SetterId
            );

        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("{problemId:guid}/testcases")]
    public async Task<IActionResult> GetTestCasesAsync(
        Guid problemId,
        CancellationToken cancellationToken)
    {
        var query = new GetTestCaseQuery(problemId);
        var result = await sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }
}
