using CodeClash.Application.DTO;
using CodeClash.Application.Mapping;
using CodeClash.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeClash.API.Controllers;
[Route("users")]
[ApiController]
public class UsersController(
    ApplicationDbContext context) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUserById(string id)
    {
        UserDto? user = await context.Users
            .Where(u => u.Id == id)
            .Select(UserMappings.ProjectToDto())
            .FirstOrDefaultAsync();

        return user is null ? NotFound() : Ok(user);
    }
}
