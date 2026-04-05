using CodeClash.Application.Abstractions.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeClash.API.Controllers.Authorization;
[Route("roles")]
[ApiController]
[Authorize(Roles = "Admin")]  //  Only admins can manage roles
public class RolesController(
    IRoleService roleService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] string roleName)
    {
        var role = await roleService.CreateRoleAsync(roleName);
        return CreatedAtAction(nameof(GetRoleByName), new { name = role.Name }, role);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await roleService.GetAllRolesAsync();
        return Ok(roles);
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> GetRoleByName(string name)
    {
        var role = await roleService.GetRoleByNameAsync(name);
        return Ok(role);
    }

    [HttpPost("assign")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
    {
        await roleService.AssignRoleAsync(request.UserId, request.RoleName);
        return Ok(new { message = $"Role '{request.RoleName}' assigned successfully." });
    }
}
