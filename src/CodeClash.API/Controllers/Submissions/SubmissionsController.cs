using System.Security.Claims;
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
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            return Unauthorized();
        }

        var query = new GetProblemSubmissionsQuery(problemId, userId);

        var response = await sender.Send(query);

        return response.IsSuccess
            ? Ok(response.Value)
            : response.Error!.Code switch
            {
                "Auth.Error" => Forbid(),
                _ => NotFound()
            };
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSubmissionData(Guid id)
    {
        var query = new GetSubmissionDataQuery(id);

        var response = await sender.Send(query);

        return response.IsSuccess ? Ok(response.Value) : NotFound();
    }
}
