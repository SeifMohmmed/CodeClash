using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Blogs;
using CodeClash.Domain.Models.Problems;
using Microsoft.EntityFrameworkCore;

namespace CodeClash.Infrastructure.Repositories;
internal sealed class BlogRepository(
    ApplicationDbContext context) : IBlogRepository
{
    public Task AddProblemToBlogAsync(
        Guid blogId,
        Guid problemId)
    {
        throw new NotImplementedException();
    }

    public Task AddSolutionToBlogAsync(
        Guid blogId,
        string solutionContent)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Blog>> GetBlogsByProblemIdAsync(Guid problemId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Problem>> GetProblemsForBlogAsync()
    {
        return await context.Problems.ToListAsync();
    }
}
