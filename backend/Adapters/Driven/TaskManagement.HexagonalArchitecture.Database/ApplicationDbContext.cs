using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManagement.HexagonalArchitecture.Domain.Entities.v1;

namespace TaskManagement.HexagonalArchitecture.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<User, Role, Guid>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>()
            .HasMany(e => e.Assignments)
            .WithOne(c => c.User);

        builder.Entity<User>()
            .HasMany(e => e.Comments)
            .WithOne(c => c.User);

        builder.Entity<Assignment>()
            .HasOne(e => e.User)
            .WithMany(c => c.Assignments);
        
        builder.Entity<Comment>()
            .HasOne(e => e.User)
            .WithMany(c => c.Comments);
        
        builder.Entity<Comment>()
            .HasOne(e => e.Assignment)
            .WithMany(c => c.Comments)
            .OnDelete(DeleteBehavior.NoAction);
        
        base.OnModelCreating(builder);
    }
}