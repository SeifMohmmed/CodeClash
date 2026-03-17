using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.TestCases;

namespace CodeClash.Infrastructure.Repositories;
internal sealed class TestCaseRepository : GenericRepository<Testcase>, ITestCaseRepository
{
    public TestCaseRepository(ApplicationDbContext context) : base(context)
    {
    }
}
