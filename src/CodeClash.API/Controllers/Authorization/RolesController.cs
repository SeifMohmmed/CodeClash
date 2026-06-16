using CodeClash.Application.Authorization.AssignRole;
using CodeClash.Application.Authorization.CreateRole;
using CodeClash.Application.Authorization.GetAllRoles;
using CodeClash.Application.Authorization.GetRoleByName;
using CodeClash.Domain.Premitives;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeClash.API.Controllers.Authorization;
/// <summary>
/// Role management endpoints.
/// </summary>
[Route("roles")]
[ApiController]
[Authorize(Roles = Roles.Admin)]
public class RolesController(ISender sender) : ControllerBase
{
    /// <summary>Create a new role.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateRole(
        [FromBody] string roleName,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateRoleCommand(roleName),
            cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetRoleByName), new { name = result.Value.Name }, result.Value)
            : BadRequest(result);
    }

    /// <summary>Get all roles.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllRoles(
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetAllRolesQuery(),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result);
    }

    /// <summary>Get role by name.</summary>
    [HttpGet("{name}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRoleByName(
        string name,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetRoleByNameQuery(name),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result)
            : NotFound(result);
    }

    /// <summary>Assign role to a user.</summary>
    [HttpPost("assign")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AssignRole(
        [FromBody] AssignRoleRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new AssignRoleCommand(request.UserId, request.RoleName),
            cancellationToken);

        return result.IsSuccess
            ? Ok()
            : BadRequest(result);
    }
}
