using CodeClash.API.Controllers.Users;
using CodeClash.Application.EditUserDetails;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeClash.API.Controllers;
/// <summary>
/// User management endpoints.
/// </summary>
[Route("users")]
[ApiController]
public class UsersController(
    ISender sender) : ControllerBase
{
    /// <summary>Update user profile details.</summary>
    [Authorize]
    [HttpPut("edit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
