using CodeClash.Application.Abstractions.CurrentUser;
using CodeClash.Application.Contests.AddContestProblems;
using CodeClash.Application.Contests.CreateContest;
using CodeClash.Application.Contests.DeleteContest;
using CodeClash.Application.Contests.GetAllContests;
using CodeClash.Application.Contests.GetContest;
using CodeClash.Application.Contests.GetContestStanding;
using CodeClash.Application.Contests.RegisterInContest;
using CodeClash.Application.Contests.UpdateContest;
using CodeClash.Application.Plagiarism.GetContestPlagiarismCases;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Premitives;
using CodeClash.Domain.Premitives.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeClash.API.Controllers.Contests;

[Route("contests")]
[ApiController]
public class ContestController(
    ISender sender,
    ICurrentUserService currentUserService) : ControllerBase
{
    [Authorize]
    [HttpGet("{id:guid}/problems")]
    [ProducesResponseType(typeof(IReadOnlyList<ContestProblemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
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
    [HttpGet]
    public async Task<IActionResult> GetAllContests(
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetAllContestsQuery(currentUserService.IdentityId!),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result);
    }


    [Authorize]
    [HttpPost]
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

    [Authorize(Roles = Roles.Admin)]
    [HttpPut("{id:guid}")]
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

    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteContest(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteContestCommand(id), cancellationToken);

        return result.IsSuccess
            ? NoContent()
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
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : BadRequest(result);
    }


    [Authorize(Roles = Roles.Admin)]
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
            "Contest.NotFound" => NotFound(result),
            "Problem.NotFound" => NotFound(result.Error),
            "Contest.Forbidden" => Forbid(),
            "Contest.Locked" => BadRequest(result),
            "Contest.ProblemLimitReached" => BadRequest(result),
            "Contest.DuplicateProblem" => Conflict(result),
            _ => BadRequest(result)
        };
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpGet("{contestId:guid}/plagiarisms")]
    [ProducesResponseType(typeof(GetContestPlagiarismCasesResponse), StatusCodes.Status200OK)]
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

    [Authorize]
    [HttpGet("{contestId:guid}/standing")]
    [ProducesResponseType(typeof(IReadOnlyList<StandingDto>), StatusCodes.Status200OK)]
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
