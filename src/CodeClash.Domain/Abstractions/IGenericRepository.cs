using System.Linq.Expressions;
using CodeClash.Domain.Premitives;
using Microsoft.EntityFrameworkCore.Storage;

namespace CodeClash.Domain.Abstractions;
public interface IGenericRepository<T> where T : BaseEntity
{
    void DeleteRange(ICollection<T> entities);

    void Delete(T entity);

    Task<T> GetByIdAsync(int id);

    Task SaveChangesAsync();

    IDbContextTransaction BeginTransaction();

    IQueryable<T> GetTableAsNotTracked();

    IQueryable<T> GetTableAsTracked();

    Task AddAsync(T entity);

    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

    Task AddRangeAsync(ICollection<T> entities);

    void Update(T entity);

    void UpdateRange(ICollection<T> entities);

    void Commit();

    void RollBack();
}
