using CodeClash.Application.Emails.SendEmails;
using CodeClash.Domain.Premitives;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeClash.API.Controllers.Emails;

[Route("email")]
[ApiController]
public class EmailController(
    ISender sender) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> SendEmail(
        [FromBody] SendEmailCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok()
            : BadRequest(result.Error);
    }
}
