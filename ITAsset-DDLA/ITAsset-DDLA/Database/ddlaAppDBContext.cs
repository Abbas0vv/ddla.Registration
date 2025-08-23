using ddla.ITApplication.Database.Models.DomainModels;
using ddla.ITApplication.Database.Models.DomainModels.Account;
using ITAsset_DDLA.Database.Models.DomainModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ddla.ITApplication.Database;

public class ddlaAppDBContext : IdentityDbContext<ddlaUser>
{
    public ddlaAppDBContext(DbContextOptions<ddlaAppDBContext> options) : base(options) { }

    public DbSet<Transfer> Transfers { get; set; }
    public DbSet<StockProduct> StockProducts { get; set; }
    public DbSet<UserPermission> UserPermissions { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<ActivityLog> ActivityLogs { get; set; }

}
