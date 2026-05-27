using Microsoft.AspNetCore.Mvc;

namespace CodeClash.API.Controllers.Blogs;
[Route("blogs")]
[ApiController]
public class BlogsController : ControllerBase
{
    //[HttpGet]
    //[Authorize]
    //public async Task<ActionResult<IEnumerable<ProblemResponse>>> GetProblemsForBlog()
    //{
    //    var problems = await blogRepository.GetProblemsForBlogAsync();

    //    var response = problems
    //                .Select(p => p.ToResponse())
    //                .ToList();

    //    return Ok(response);
    //}
}
