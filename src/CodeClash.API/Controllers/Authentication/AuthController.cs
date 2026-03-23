using CodeClash.Application.Authentication.Login;
using CodeClash.Application.Authentication.Register;
using CodeClash.Application.DTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeClash.API.Controllers.Authentication;
[Route("auth")]
[ApiController]
[AllowAnonymous]
public sealed class AuthController(
    ISender sender) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AccessTokenDto>> Register(RegisterUserDto dto)
    {
        var command = new RegisterUserCommand(
            dto.Email,
            dto.Password,
            dto.Name);

        var result = await sender.Send(command);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AccessTokenDto>> Login(LoginUserDto dto)
    {
        var query = new LoginQuery(dto.Email, dto.Password);

        var result = await sender.Send(query);

        if (result.IsFailure)
        {
            return Unauthorized(result.Error);
        }

        return Ok(result.Value);
    }
}
