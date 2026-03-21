using CodeClash.Application.Authentication.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CodeClash.API.Controllers.Authentication;
[Route("auth")]
[ApiController]
public class AuthController(
    ISender sender) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserDto dto)
    {
        var command = new RegisterUserCommand(
            dto.Email,
            dto.Password,
            dto.Name);

        var result = await sender.Send(command);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
