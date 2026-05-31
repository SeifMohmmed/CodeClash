using CodeClash.Application.Abstractions.Plagiarism;
using CodeClash.Domain.Requests;
using Microsoft.AspNetCore.Mvc;

namespace CodeClash.API.Controllers.Plagiarisms;

[ApiController]
[Route("plagiarisms")]
public sealed class PlagiarismController(
    IPlagiarismService plagiarismService) : ControllerBase
{
    [HttpPost("similarity")]
    [ProducesResponseType(typeof(CodeSimilarityRequest), StatusCodes.Status200OK)]
    public IActionResult GetSimilarity([FromBody] CodeSimilarityRequest request)
    {
        var similarity = plagiarismService.GetSimilarity(request.Code1, request.Code2);
        return Ok(similarity);
    }
}
