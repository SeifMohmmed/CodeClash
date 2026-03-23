using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CodeClash.Application.Abstractions.Identity;
public interface IAppDbContext
{
    DatabaseFacade Database { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
