using CodeClash.Domain.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CodeClash.Application.Abstractions.Identity;

public interface IAppDbContext
{
    DatabaseFacade Database { get; }
    DbSet<User> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
