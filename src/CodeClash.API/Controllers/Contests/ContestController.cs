using System.Security.Claims;
using CodeClash.Application.Contests.AddContestProblems;
using CodeClash.Application.Contests.CreateContest;
using CodeClash.Application.Contests.GetAllContests;
using CodeClash.Application.Contests.GetContest;
using CodeClash.Application.Contests.RegisterInContest;
using CodeClash.Application.Plagiarism.GetContestPlagiarismCases;
using CodeClash.Domain.Premitives.Responses;
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
    [ProducesResponseType(typeof(ContestProblemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ContestProblemResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetContestProblems(
    Guid id,
    CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetContestQuery(id), cancellationToken);

        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllContests(
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = await sender.Send(new GetAllContestsQuery(userId!), cancellationToken);

        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result.Error);
    }


    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateContest(
    [FromBody] CreateContestCommand command,
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

    [Authorize]
    [HttpPost("{contestId:guid}/problems/{problemId:guid}")]
    public async Task<IActionResult> AddProblem(
        [FromRoute] Guid contestId,
        [FromRoute] Guid problemId,
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

    [Authorize]
    [HttpGet("{contestId:guid}/plagiarisms")]
    [ProducesResponseType(typeof(GetContestPlagiarismCasesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPlagiarismCases(
    [FromRoute] Guid contestId,
    [FromQuery] decimal threshold,
    [FromQuery] List<Guid> problemIds,
    CancellationToken cancellationToken)
    {
        var query = new GetContestPlagiarismCasesQuery(contestId, threshold, problemIds);
        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result)
            : NotFound(result);
    }
}
