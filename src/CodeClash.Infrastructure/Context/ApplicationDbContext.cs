using CodeClash.Domain.Models.Identity;
using CodeClash.Domain.Models.Problems;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CodeClash.Infrastructure.Context;
public sealed class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Problem> Problems { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Automatically apply IEntityTypeConfiguration<T>
        builder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(builder);
    }

}
