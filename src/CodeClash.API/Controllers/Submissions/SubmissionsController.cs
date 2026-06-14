using CodeClash.Application.Submissions.GetProblemSubmissions;
using CodeClash.Application.Submissions.GetSubmissionData;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeClash.API.Controllers.Submissions;

[Route("submissions")]
[ApiController]
[Authorize]
public class SubmissionsController(
    ISender sender) : ControllerBase
{
    [HttpGet("problem/{problemId}")]
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

    [HttpGet("{id:guid}")]
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
