using CodeClash.Domain.Models.Topics;

namespace CodeClash.Domain.Abstractions;
public interface ITopicRepository : IGenericRepository<Topic>
{
    Task<List<Guid>> GetExistingIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken);

    Task<bool> ExistsAsync(
        string name,
        CancellationToken cancellationToken);

    Task<List<string>> GetTopicNamesByIdsAsync(
        IEnumerable<Guid>
        topicIds);
    Task<List<Guid>> GetTopicIDsByNamesAsync(
        IEnumerable<string> topicsNames);
}
