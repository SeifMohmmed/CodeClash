using CodeClash.Application.Plagiarism.GetCodeSimilarity;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Premitives;
using CodeClash.Domain.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeClash.API.Controllers.Plagiarisms;

[ApiController]
[Route("plagiarisms")]
[Authorize(Roles = Roles.Admin)]
public sealed class PlagiarismController(
    ISender sender) : ControllerBase
{
    [HttpPost("similarity")]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
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
