using CodeClash.Application.Submissions.GetProblemSubmissions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CodeClash.API.Controllers.Submissions;
[Route("submissions")]
[ApiController]
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
}
