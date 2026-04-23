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
    public async Task<IActionResult> GetProblemSubmissions(Guid problemId)
    {
        var query = new GetProblemSubmissionsQuery(problemId);

        var response = await sender.Send(query);

        return response.IsSuccess ? Ok(response.Value) : NotFound();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSubmissionData(Guid submissionId)
    {
        var query = new GetSubmissionDataQuery(submissionId);

        var response = await sender.Send(query);

        return response.IsSuccess ? Ok(response.Value) : NotFound();
    }
}
