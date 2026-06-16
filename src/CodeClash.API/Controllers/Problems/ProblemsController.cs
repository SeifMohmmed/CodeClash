using CodeClash.Application.Abstractions.CurrentUser;
using CodeClash.Application.Problems.CreateProblem;
using CodeClash.Application.Problems.DeleteProblem;
using CodeClash.Application.Problems.GetAll;
using CodeClash.Application.Problems.GetPrblemTestcases;
using CodeClash.Application.Problems.GetProblemById;
using CodeClash.Domain.Premitives;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeClash.API.Controllers.Problems;
/// <summary>
/// Problem management endpoints.
/// </summary>
[Route("problems")]
[ApiController]
public class ProblemsController(
    ISender sender,
    ICurrentUserService currentUserService) : ControllerBase
{
    /// <summary>Create a problem.</summary>
    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
            currentUserService.IdentityId!
            );

        var result = await sender.Send(command, cancellationToken);

        return result.IsFailure
            ? BadRequest(result)
            : Ok(result);
    }

    /// <summary>Get all problems.</summary>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProblems(
            [FromQuery] List<Guid>? topics,
            [FromQuery] string? problemName,
            [FromQuery] ProblemStatus? status,
            [FromQuery] Difficulty? difficulty,
            [FromQuery] Order order = Order.Ascending,
            [FromQuery] SortBy sortBy = SortBy.Name,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
    {
        var query = new GetAllProblemsQuery(
            topics,
            problemName,
            difficulty,
            status,
            sortBy,
            order,
            pageNumber,
            pageSize);

        var result = await sender.Send(query);

        return result.IsFailure
            ? NotFound(result)
            : Ok(result);
    }

    /// <summary>Get problem details.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProblemDetails(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetProblemByIdQuery(id), cancellationToken);

        return result.IsSuccess
            ? Ok(result)
            : NotFound(result);
    }

    /// <summary>Delete a problem.</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteProblemCommand(id), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : NotFound(result);
    }

    /// <summary>Get problem test cases.</summary>
    [HttpGet("{problemId:guid}/testcases")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTestCasesAsync(
    Guid problemId,
    CancellationToken cancellationToken)
    {
        var query = new GetTestCaseQuery(problemId);
        var result = await sender.Send(query, cancellationToken);

        return result.IsFailure
            ? NotFound(result)
            : Ok(result);
    }
}
