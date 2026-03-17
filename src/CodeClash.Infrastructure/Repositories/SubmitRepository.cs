using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Submits;

namespace CodeClash.Infrastructure.Repositories;
internal sealed class SubmitRepository : GenericRepository<Submit>, ISubmitRepository
{
    public SubmitRepository(ApplicationDbContext context)
        : base(context)
    {
    }
}
