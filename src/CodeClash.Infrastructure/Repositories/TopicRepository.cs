using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Topics;
using Microsoft.EntityFrameworkCore;

namespace CodeClash.Infrastructure.Repositories;
internal sealed class TopicRepository : GenericRepository<Topic>, ITopicRepository
{
    private readonly ApplicationDbContext _context;
    public TopicRepository(
        ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
    public async Task<List<Guid>> GetExistingIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        return await _context.Topics
            .Where(t => ids.Contains(t.Id))
            .Select(t => t.Id)
            .ToListAsync(cancellationToken);
    }
}
