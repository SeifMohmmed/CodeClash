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

    public async Task<bool> ExistsAsync(
        string name,
        CancellationToken cancellationToken)
    {
        return await _context.Topics
            .AnyAsync(t => t.Name == name, cancellationToken);
    }

    public async Task<List<Guid>> GetExistingIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        return await _context.Topics
            .Where(t => ids.Contains(t.Id))
            .Select(t => t.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Guid>> GetTopicIDsByNamesAsync(
        IEnumerable<string> topicsNames)
    {
        return await _context.Topics
            .Where(x => topicsNames.Contains(x.Name))
            .Select(x => x.Id)
            .ToListAsync();
    }

    public async Task<List<string>> GetTopicNamesByIdsAsync(
        IEnumerable<Guid> topicIds)
    {
        return await _context.Topics
            .Where(x => topicIds.Contains(x.Id))
            .Select(x => x.Name)
            .ToListAsync();
    }
}
