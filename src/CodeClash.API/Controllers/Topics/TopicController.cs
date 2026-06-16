using CodeClash.Application.Topics.CreateTopics;
using CodeClash.Application.Topics.GetAllTopics;
using CodeClash.Domain.Premitives;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeClash.API.Controllers.Topics;
/// <summary>
/// Topic management endpoints.
/// </summary>
[Route("topics")]
[ApiController]
public sealed class TopicController(
    ISender sender) : ControllerBase
{
    /// <summary>Get all topics.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllTopics(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllTopicsQuery(), cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result);
    }

    /// <summary>Create a topic.</summary>
    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTopic(
    [FromBody] CreateTopicCommand command,
    CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        return result.IsFailure
            ? BadRequest(result.Error)
            : CreatedAtAction(nameof(GetAllTopics), new { id = result.Value.Id }, result.Value);
    }
}
