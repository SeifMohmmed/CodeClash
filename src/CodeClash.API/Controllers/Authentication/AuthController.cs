using CodeClash.Application.Authentication.ConfirmEmail;
using CodeClash.Application.Authentication.Login;
using CodeClash.Application.Authentication.RefreshTokens;
using CodeClash.Application.Authentication.Register;
using CodeClash.Application.Authentication.ResendConfirmationEmail;
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

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenDto dto)
    {
        var command = new RefreshTokenCommand(dto.RefreshToken);

        var result = await sender.Send(command);

        if (result.IsFailure)
        {
            return Unauthorized(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(
    [FromQuery] string userId,
    [FromQuery] string token,
    CancellationToken cancellationToken)
    {
        var command = new ConfirmEmailCommand(userId, token);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }

    [HttpPost("resend-confirmation-email")]
    public async Task<IActionResult> ResendConfirmationEmail(
    [FromBody] ResendConfirmationEmailCommand command)
    {
        var result = await sender.Send(command);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }
}
