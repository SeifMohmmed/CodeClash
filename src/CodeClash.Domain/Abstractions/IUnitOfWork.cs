using CodeClash.Domain.Premitives;

namespace CodeClash.Domain.Abstractions;
public interface IUnitOfWork
{
    IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
    Task<int> CompleteAsync();
}
