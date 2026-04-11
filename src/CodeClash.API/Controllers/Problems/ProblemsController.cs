using System.Security.Claims;
using CodeClash.Application.Problems.CreateProblem;
using CodeClash.Application.Problems.DeleteProblem;
using CodeClash.Application.Problems.GetAll;
using CodeClash.Application.Problems.GetPrblemTestcases;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeClash.API.Controllers.Problems;
[Route("problems")]
[ApiController]
public class ProblemsController(
    ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
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
            request.Topics,
            request.SetterId
            );

        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetProblems(
        [FromQuery] GetAllProblemsRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var query = new GetAllProblemsQuery(
            UserId: userId,
            Name: request.Name ?? string.Empty,
            TopicsIds: request.TopicsIds,
            Difficulty: request.Difficulty
        );

        var result = await sender.Send(query, cancellationToken);


        return result.IsFailure
            ? NotFound(result.Error)
            : Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteProblemCommand(id), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : NotFound(result.Error);
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
