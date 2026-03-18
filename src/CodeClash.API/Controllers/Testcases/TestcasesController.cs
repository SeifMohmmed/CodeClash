using CodeClash.Application.TestCase.CreateTestcases;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CodeClash.API.Controllers.Testcases;
[Route("testcase")]
[ApiController]
public class TestcasesController(
    ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateTestcaseAsync(CreateTestcaseQuery command)
    {
        var result = await sender.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }



}
