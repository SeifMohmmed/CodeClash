using System.Linq.Expressions;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Premitives;
using Microsoft.EntityFrameworkCore;

namespace CodeClash.Infrastructure.Repositories;
internal class GenericRepository<T>(
    ApplicationDbContext context)
    : IGenericRepository<T> where T : Entity
{
    #region Methods
    public async Task<T?> GetByIdAsync(int id)
        => await context.Set<T>().FindAsync(id);

    public async Task<T?> GetByIdAsync(Guid id)
       => await context.Set<T>().FindAsync(id);

    public async Task<bool> AnyAsync(Expression<Func<T?, bool>> predicate)
    => await context.Set<T>().AnyAsync(predicate);

    public void Add(T entity)
    => context.Set<T>().Add(entity);

    public void AddRange(ICollection<T> entities)
       => context.Set<T>().AddRange(entities);
    public void Update(T entity)
    => context.Set<T>().Update(entity);

    public void UpdateRange(ICollection<T> entities)
        => context.Set<T>().UpdateRange(entities);

    public void Delete(T entity)
       => context.Set<T>().Remove(entity);

    public void DeleteRange(ICollection<T> entities)
       => context.Set<T>().RemoveRange(entities);

    #endregion
}
