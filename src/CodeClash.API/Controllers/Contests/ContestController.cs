using CodeClash.Application.Contests.CreateContest;
using CodeClash.Application.Contests.GetContest;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeClash.API.Controllers.Contests;

[Route("contests")]
[ApiController]
public class ContestController(
    ISender sender) : ControllerBase
{
    [HttpGet("{id:guid}/problems")]
    public async Task<IActionResult> GetContestProblems(
    Guid id,
    CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetContestQuery(id), cancellationToken);

        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateContest(
    CreateContestCommand command,
    CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(
                nameof(GetContestProblems),
                new { id = result.Value.Id },
                result)
            : BadRequest(result);
    }
}
