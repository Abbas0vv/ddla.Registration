using ddla.ITApplication.Database.Models.DomainModels;
using ddla.ITApplication.Database.Models.DomainModels.Account;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ddla.ITApplication.Database;

public class ddlaAppDBContext : IdentityDbContext<ddlaUser>
{
    public ddlaAppDBContext(DbContextOptions<ddlaAppDBContext> options) : base(options) { }

    public DbSet<Department> Departments { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Unit> Units { get; set; }
}
