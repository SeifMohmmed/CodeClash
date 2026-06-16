using CodeClash.Application.Submissions.GetProblemSubmissions;
using CodeClash.Application.Submissions.GetSubmissionData;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeClash.API.Controllers.Submissions;

/// <summary>
/// Submission retrieval endpoints.
/// </summary>
[Route("submissions")]
[ApiController]
[Authorize]
public class SubmissionsController(
    ISender sender) : ControllerBase
{
    /// <summary>Get submissions for a problem.</summary>
    [HttpGet("problem/{problemId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProblemSubmissions(
        Guid problemId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetProblemSubmissionsQuery(problemId),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result)
            : NotFound(result);
    }

    /// <summary>Get submission details.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSubmissionData(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetSubmissionDataQuery(id), cancellationToken);

        return result.IsSuccess
            ? Ok(result)
            : NotFound(result);
    }
}
