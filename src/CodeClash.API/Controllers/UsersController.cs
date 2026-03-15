using CodeClash.API.Helpers;
using CodeClash.Application.Authentication.Login;
using CodeClash.Application.Authentication.Register;
using Microsoft.AspNetCore.Mvc;

namespace CodeClash.API.Controllers;
[Route("users")]
[ApiController]
public class UsersController : BaseController
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand command)
    {
        return ResponseResult(await mediator.Send(command));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginQuery query)
    {
        return ResponseResult(await mediator.Send(query));
    }
}
