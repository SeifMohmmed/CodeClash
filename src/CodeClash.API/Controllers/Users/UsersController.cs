using System.Security.Claims;
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
    //[HttpGet("{id}")]
    //public async Task<ActionResult<UserDto>> GetUserById(string id)
    //{
    //    UserDto? user = await context.Users
    //        .Where(u => u.Id == id)
    //        .Select(UserMappings.ProjectToDto())
    //        .FirstOrDefaultAsync();

    //    return user is null ? NotFound() : Ok(user);
    //}

    [HttpPut("edit")]
    [Authorize]
    public async Task<IActionResult> EditUserDetails(
    [FromForm] EditUserDetailsRequest request,
    CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = await sender.Send(new EditUserDetailsQuery(
            userId!,
            request.Name,
            request.Image,
            request.Gender), cancellationToken);

        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result);
    }
}
