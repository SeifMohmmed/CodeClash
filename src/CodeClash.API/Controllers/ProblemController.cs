using CodeClash.API.Helpers;
using CodeClash.Application.Problems.CreateProblem;
using CodeClash.Domain.Models.Problems;
using CodeClash.Domain.Premitives;
using Microsoft.AspNetCore.Mvc;

namespace CodeClash.API.Controllers;
[Route("problem")]
[ApiController]
public class ProblemController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> CreateProblemAsync(CreateProblemCommand command)
    {
        Result<Problem> response = await mediator.Send(command);

        return ResponseResult(response);
    }
}
