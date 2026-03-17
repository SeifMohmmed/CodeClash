using CodeClash.Domain.Models.Problems;
using CodeClash.Domain.Models.TestCases;

namespace CodeClash.Domain.Abstractions;
public interface IProblemRepository : IGenericRepository<Problem>
{
    Task<IEnumerable<Testcase>> GetTestCasesByProblemId(Guid problemId);
}
