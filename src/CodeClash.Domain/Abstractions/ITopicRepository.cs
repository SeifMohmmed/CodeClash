using CodeClash.Domain.Models.Topics;

namespace CodeClash.Domain.Abstractions;
public interface ITopicRepository : IGenericRepository<Topic>
{
    Task<List<Guid>> GetExistingIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken);

}
