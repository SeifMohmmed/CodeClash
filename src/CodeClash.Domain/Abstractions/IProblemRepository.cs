using CodeClash.Domain.Models.Problems;
using CodeClash.Domain.Models.TestCases;

namespace CodeClash.Domain.Abstractions;
public interface IProblemRepository : IGenericRepository<Problem>
{
    IQueryable<Testcase> GetTestCasesByProblemId(Guid problemId);
}
