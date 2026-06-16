using CodeClash.Application.TestCase.CreateTestcases;
using CodeClash.Application.TestCase.DeleteTestcases;
using CodeClash.Application.TestCase.UpdateTestcases;
using CodeClash.Domain.Premitives;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeClash.API.Controllers.Testcases;
/// <summary>
/// Test case management endpoints.
/// </summary>
[Route("testcases")]
[ApiController]
[Authorize(Roles = Roles.Admin)]
public class TestcasesController(
    ISender sender) : ControllerBase
{
    /// <summary>Create a test case.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTestcaseAsync(
       [FromBody] CreateTestcaseRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateTestcaseCommand(
            request.ProblemId,
            request.Input,
            request.Output);

        var result = await sender.Send(command, cancellationToken);

        return result.IsFailure
            ? BadRequest(result.Error)
            : CreatedAtAction(nameof(CreateTestcaseAsync), new { id = result.Value }, result.Value);
    }

    /// <summary>Update a test case.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTestcaseAsync(
         Guid id,
         [FromBody] UpdateTestcaseRequest request,
         CancellationToken cancellationToken)
    {
        var command = new UpdateTestcaseCommand(id, request.Input, request.Output);
        var result = await sender.Send(command, cancellationToken);

        return result.IsFailure
            ? NotFound(result)
            : NoContent();
    }

    /// <summary>Delete a test case.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTestcaseAsync(
    Guid id,
    CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteTestcaseCommand(id), cancellationToken);

        return result.IsFailure
            ? NotFound(result)
            : NoContent();
    }
}
