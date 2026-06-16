using CodeClash.Application.Plagiarism.GetCodeSimilarity;
using CodeClash.Domain.Premitives;
using CodeClash.Domain.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeClash.API.Controllers.Plagiarisms;

/// <summary>
/// Plagiarism detection endpoints.
/// </summary>
[ApiController]
[Route("plagiarisms")]
[Authorize(Roles = Roles.Admin)]
public sealed class PlagiarismController(ISender sender) : ControllerBase
{
    /// <summary>Calculate similarity between two code snippets.</summary>
    [HttpPost("similarity")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSimilarity(
        [FromBody] CodeSimilarityRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetCodeSimilarityQuery(request.Code1, request.Code2),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result);
    }
}
