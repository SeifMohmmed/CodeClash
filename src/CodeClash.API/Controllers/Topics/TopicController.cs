using CodeClash.Application.Topics.CreateTopics;
using CodeClash.Application.Topics.GetAllTopics;
using CodeClash.Domain.Premitives;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeClash.API.Controllers.Topics;

[Route("topics")]
[ApiController]
public sealed class TopicController(
    ISender sender) : ControllerBase
{
    [HttpGet]
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

    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
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
