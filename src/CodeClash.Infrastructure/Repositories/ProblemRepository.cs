using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Problems;
using CodeClash.Domain.Models.TestCases;

namespace CodeClash.Infrastructure.Repositories;
internal sealed class ProblemRepository : GenericRepository<Problem>, IProblemRepository
{
    private readonly ApplicationDbContext _context;
    public ProblemRepository(ApplicationDbContext context)
        : base(context)
    {
        _context = context;
    }

    public IQueryable<Testcase> GetTestCasesByProblemId(Guid problemId)
    {
        return _context.Set<Testcase>()
                .Where(x => x.ProblemId == problemId);
    }
}
