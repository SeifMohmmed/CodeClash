using CodeClash.Application.Emails.SendEmails;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CodeClash.API.Controllers.Emails;
[Route("email")]
[ApiController]
public class EmailController(
    ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SendEmail([FromBody] SendEmailCommand command)
    {
        var response = await sender.Send(command);

        if (response.IsFailure)
        {
            return BadRequest(response.Error);
        }

        return Ok();
    }
}
