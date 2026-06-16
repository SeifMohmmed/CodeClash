using CodeClash.Application.Contests.AddContestProblems;
using CodeClash.Application.Contests.CreateContest;
using CodeClash.Application.Contests.DeleteContest;
using CodeClash.Application.Contests.GetAllContests;
using CodeClash.Application.Contests.GetContest;
using CodeClash.Application.Contests.GetContestStanding;
using CodeClash.Application.Contests.RegisterInContest;
using CodeClash.Application.Contests.UpdateContest;
using CodeClash.Application.Plagiarism.GetContestPlagiarismCases;
using CodeClash.Domain.Premitives;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeClash.API.Controllers.Contests;
/// <summary>
/// Contest management endpoints.
/// </summary>
[Route("contests")]
[ApiController]
public class ContestController(
    ISender sender) : ControllerBase
{
    /// <summary>Get contest problems.</summary>
    [Authorize]
    [HttpGet("{id:guid}/problems")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetContestProblems(
    Guid id,
    CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetContestQuery(id), cancellationToken);

        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result);
    }

    /// <summary>Get all contests.</summary>
    [Authorize]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllContests(
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetAllContestsQuery(),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result);
    }


    /// <summary>Create a contest.</summary>
    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateContest(
    [FromBody] CreateContestRequest request,
    CancellationToken cancellationToken)
    {
        var command = new CreateContestCommand(
            request.Name,
            request.Description,
            request.StartTime,
            request.EndTime);

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(
                nameof(GetContestProblems),
                new { id = result.Value.Id },
                result)
            : BadRequest(result);
    }

    /// <summary>Update contest details.</summary>
    [Authorize(Roles = Roles.Admin)]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateContest(
    [FromRoute] Guid id,
    [FromBody] UpdateContestRequest request,
    CancellationToken cancellationToken)
    {
        var command = new UpdateContestCommand(
            id,
            request.Name,
            request.StartDate,
            request.EndDate);

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result);
    }

    /// <summary>Delete a contest.</summary>
    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteContest(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteContestCommand(id), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(result);
    }

    /// <summary>Register in a contest.</summary>
    [Authorize]
    [HttpPost("{contestId:guid}/register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterInContest(
    Guid contestId,
    CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RegisterInContestCommand(contestId), cancellationToken);

        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : BadRequest(result);
    }

    /// <summary>Add a problem to a contest.</summary>
    [Authorize(Roles = Roles.Admin)]
    [HttpPost("{contestId:guid}/problems/{problemId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
            "Contest.NotFound" => NotFound(result),
            "Problem.NotFound" => NotFound(result.Error),
            "Contest.Forbidden" => Forbid(),
            "Contest.Locked" => BadRequest(result),
            "Contest.ProblemLimitReached" => BadRequest(result),
            "Contest.DuplicateProblem" => Conflict(result),
            _ => BadRequest(result)
        };
    }

    /// <summary>Get plagiarism cases for a contest.</summary>
    [Authorize(Roles = Roles.Admin)]
    [HttpGet("{contestId:guid}/plagiarisms")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPlagiarismCases(
    [FromRoute] Guid contestId,
    [FromQuery] decimal threshold,
    [FromQuery] List<Guid> problemIds,
    CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetContestPlagiarismCasesQuery(contestId, threshold, problemIds),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result);
    }

    /// <summary>Get contest standings.</summary>
    [Authorize]
    [HttpGet("{contestId:guid}/standing")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetContestStanding(
    [FromRoute] Guid contestId,
    [FromQuery] int start,
    [FromQuery] int pageSize,
    CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetContestStandingQuery(contestId, start, pageSize),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result)
            : NotFound(result);
    }
}
