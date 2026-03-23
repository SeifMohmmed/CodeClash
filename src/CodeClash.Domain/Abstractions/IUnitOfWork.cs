using Microsoft.EntityFrameworkCore.Storage;

namespace CodeClash.Domain.Abstractions;
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<IDbContextTransaction> BeginTransactionAsync(
    CancellationToken cancellationToken = default);
}
