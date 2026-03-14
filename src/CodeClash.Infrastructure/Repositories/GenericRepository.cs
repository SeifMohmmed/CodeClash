using System.Linq.Expressions;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Premitives;
using CodeClash.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CodeClash.Infrastructure.Repositories;
internal sealed class GenericRepository<T>(
    ApplicationDbContext context)
    : IGenericRepository<T> where T : BaseEntity
{
    #region Methods
    public async Task AddAsync(T entity)
    => await context.Set<T>().AddAsync(entity);

    public async Task AddRangeAsync(ICollection<T> entities)
       => await context.Set<T>().AddRangeAsync(entities);

    public IDbContextTransaction BeginTransaction()
        => context.Database.BeginTransaction();

    public void Commit()
        => context.Database.CommitTransaction();

    public void Delete(T entity)
       => context.Set<T>().Remove(entity);

    public void DeleteRange(ICollection<T> entities)
       => context.Set<T>().RemoveRange(entities);

    public async Task<T> GetByIdAsync(int id)
        => await context.Set<T>().FindAsync(id);

    public IQueryable<T> GetTableAsTracked()
        => context.Set<T>().AsQueryable();

    public IQueryable<T> GetTableAsNotTracked()
        => context.Set<T>().AsNoTracking().AsQueryable();

    public void RollBack()
        => context.Database.RollbackTransaction();

    public async Task SaveChangesAsync()
        => await context.SaveChangesAsync();

    public void Update(T entity)
        => context.Set<T>().Update(entity);

    public void UpdateRange(ICollection<T> entities)
        => context.Set<T>().UpdateRange(entities);

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        => await context.Set<T>().AnyAsync(predicate);

    #endregion
}
