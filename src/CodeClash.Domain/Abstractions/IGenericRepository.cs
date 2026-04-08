using System.Linq.Expressions;
using CodeClash.Domain.Premitives;

namespace CodeClash.Domain.Abstractions;
public interface IGenericRepository<T> where T : Entity
{
    Task<T?> GetByIdAsync(int id);

    Task<T?> GetByIdAsync(Guid id);

    Task<bool> AnyAsync(
        Expression<Func<T?, bool>> predicate);

    void Add(T entity);

    void AddRange(ICollection<T> entities);

    void Update(T entity);

    void UpdateRange(ICollection<T> entities);

    void DeleteRange(ICollection<T> entities);

    void Delete(T entity);
}
