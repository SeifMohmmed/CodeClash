using CodeClash.API.Controllers.Users;
using CodeClash.Application.EditUserDetails;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeClash.API.Controllers;

[Route("users")]
[ApiController]
public class UsersController(
    ISender sender) : ControllerBase
{
    [Authorize]
    [HttpPut("edit")]
    public async Task<IActionResult> EditUserDetails(
    [FromForm] EditUserDetailsRequest request,
    CancellationToken cancellationToken)
    {
        var result = await sender.Send(new EditUserDetailsCommand(
            request.Name,
            request.Image,
            request.Gender), cancellationToken);

        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result);
    }
}
