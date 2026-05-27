using CodeClash.Domain.Models.Blogs;
using CodeClash.Domain.Models.Problems;

namespace CodeClash.Domain.Abstractions;
public interface IBlogRepository
{
    // Problem methods
    Task<IEnumerable<Problem>> GetProblemsForBlogAsync();
    Task AddProblemToBlogAsync(Guid blogId, Guid problemId);

    // Solution methods
    Task AddSolutionToBlogAsync(Guid blogId, string solutionContent);

    // Get blogs by problem
    Task<IEnumerable<Blog>> GetBlogsByProblemIdAsync(Guid problemId);
}
