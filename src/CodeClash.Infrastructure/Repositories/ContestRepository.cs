using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Contests;

namespace CodeClash.Infrastructure.Repositories;
internal sealed class ContestRepository : GenericRepository<Contest>, IContestRepository
{
    public ContestRepository(ApplicationDbContext context)
        : base(context)
    {
    }
}
