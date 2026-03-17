using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Submits;

namespace CodeClash.Infrastructure.Repositories;
internal sealed class SubmissionRepository : GenericRepository<Submit>, ISubmissionRepository
{
    private readonly ApplicationDbContext _context;

    public SubmissionRepository(
        ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public IQueryable<Submit> GetAllSubmissions(Guid problemId, string userId)
     => _context.Submits.Where(x => x.ProblemId == problemId && x.UserId == userId);
}
