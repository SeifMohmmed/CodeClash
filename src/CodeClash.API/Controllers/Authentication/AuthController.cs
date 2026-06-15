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
    public async Task<IActionResult> Register(
        RegisterUserDto dto,
        CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(
            dto.Email,
            dto.Password,
            dto.Name);

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginUserDto dto,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new LoginQuery(dto.Email, dto.Password),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result)
            : Unauthorized(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        RefreshTokenDto dto,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new RefreshTokenCommand(dto.RefreshToken),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result)
            : Unauthorized(result);
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(
    [FromQuery] string userId,
    [FromQuery] string token,
    CancellationToken cancellationToken)
    {
        var command = new ConfirmEmailCommand(userId, token);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result);
    }

    [HttpPost("resend-confirmation-email")]
    public async Task<IActionResult> ResendConfirmationEmail(
    [FromBody] ResendConfirmationEmailCommand command,
    CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result);
    }
}
