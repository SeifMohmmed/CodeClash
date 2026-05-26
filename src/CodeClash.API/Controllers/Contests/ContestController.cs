using CodeClash.Application.Contest.GetContest;
using MediatR;
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
}
