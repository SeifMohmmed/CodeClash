using System.Collections;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Premitives;
using CodeClash.Infrastructure.Context;
using CodeClash.Infrastructure.Repositories;

namespace CodeClash.Infrastructure.Implementation;
public sealed class UnitOfWork(
    ApplicationDbContext context) : IUnitOfWork
{
    private readonly Hashtable _repositories = new Hashtable();
    public Task<int> CompleteAsync()
               => context.SaveChangesAsync();

    public ValueTask DisposeAsync()
        => context.DisposeAsync();

    // create repository per request  
    public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
    {
        // if repository<order> => key = order
        var key = typeof(TEntity).Name;
        if (!_repositories.ContainsKey(key))
        {
            var repo = new GenericRepository<TEntity>(context);

            _repositories.Add(key, repo);
        }

        return _repositories[key] as IGenericRepository<TEntity>;
    }
}
