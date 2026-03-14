using System.Net;
using System.Security.Claims;
using CodeClash.Domain.Premitives;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeClash.API.Helpers;

/// <summary>
/// Base controller that provides common functionality
/// for all API controllers in the application.
/// 
/// It centralizes:
/// - MediatR access
/// - Standardized API responses
/// - Current authenticated user helpers
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class BaseController : ControllerBase
{
    // Lazy-loaded MediatR instance
    // This avoids injecting IMediator in every controller constructor.
    private IMediator mediatorInstance;

    /// <summary>
    /// Provides access to MediatR using lazy initialization.
    /// The instance is resolved from the request services only when needed.
    /// </summary>
    protected IMediator mediator =>
        mediatorInstance ??= HttpContext.RequestServices.GetRequiredService<IMediator>();

    /// <summary>
    /// Converts a domain Response object into a proper HTTP response.
    /// This ensures consistent API responses across the application.
    /// </summary>
    protected IActionResult ResponseResult<T>(Result<T> result)
    {
        if (result.IsFailure)
        {
            var response = new Response
            {
                StatusCode = HttpStatusCode.BadRequest,
                IsSuccess = false,
                Message = result.Error.Message
            };

            return StatusCode((int)response.StatusCode, response);
        }

        var successResponse = new Response
        {
            StatusCode = HttpStatusCode.OK,
            IsSuccess = true,
            Data = result.Value,
            Message = result.Message
        };

        return Ok(successResponse);
    }

    /// <summary>
    /// Retrieves the email of the currently authenticated user
    /// from the JWT claims.
    /// </summary
    [Authorize]
    internal string GetCurrentUserEmail()
        => User.FindFirstValue(ClaimTypes.Email);
}
