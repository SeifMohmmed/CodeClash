using CodeClash.Application.TestCase.CreateTestcases;
using CodeClash.Application.TestCase.DeleteTestcases;
using CodeClash.Application.TestCase.UpdateTestcases;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CodeClash.API.Controllers.Testcases;
[Route("testcases")]
[ApiController]
public class TestcasesController(
    ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateTestcaseAsync(
       [FromBody] CreateTestcaseRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateTestcaseCommand(
            request.ProblemId,
            request.Input,
            request.Output);

        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);

    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTestcaseAsync(
         Guid id,
         [FromBody] UpdateTestcaseRequest request,
         CancellationToken cancellationToken)
    {
        var command = new UpdateTestcaseCommand(id, request.Input, request.Output);
        var result = await sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTestcaseAsync(
    Guid id,
    CancellationToken cancellationToken)
    {
        var command = new DeleteTestcaseCommand(id);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return NoContent();
    }
}
