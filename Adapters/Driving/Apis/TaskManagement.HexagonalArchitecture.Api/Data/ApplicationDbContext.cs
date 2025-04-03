using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManagement.HexagonalArchitecture.Domain.Entities.v1;

namespace TaskManagement.HexagonalArchitecture.Api.Data
{
    internal class ApplicationDbContext : IdentityDbContext<User, Role, Guid>
    {
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Assignment> Assignments { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Ignore<Notification>();

        //    modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        //    base.OnModelCreating(modelBuilder);
        //}
    }
}
