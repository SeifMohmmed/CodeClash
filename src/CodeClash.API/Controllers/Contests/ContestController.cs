using CodeClash.Application.Contests.AddContestProblems;
using CodeClash.Application.Contests.CreateContest;
using CodeClash.Application.Contests.GetContest;
using CodeClash.Application.Contests.RegisterInContest;
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

    [Authorize]
    [HttpPost("{contestId:guid}/register")]
    public async Task<IActionResult> RegisterInContest(
    Guid contestId,
    CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RegisterInContestCommand(contestId), cancellationToken);
        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result);
    }

    [HttpPost("{contestId:guid}/problems/{problemId:guid}")]
    [Authorize]
    public async Task<IActionResult> AddProblem(
        Guid contestId,
        Guid problemId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new AddContestProblemCommand(contestId, problemId),
            cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return result.Error!.Code switch
        {
            "Auth.Unauthorized" => Unauthorized(result),
            "Contest.NotFound" => NotFound(result),
            "Contest.Forbidden" => Forbid(),
            "Contest.Locked" => BadRequest(result),
            "Contest.ProblemLimitReached" => BadRequest(result),
            "Problem.NotFound" => NotFound(result),
            "Contest.DuplicateProblem" => Conflict(result),
            _ => BadRequest(result)
        };
    }
}
