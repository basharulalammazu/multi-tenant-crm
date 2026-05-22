using DAL.EF.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DAL.EF
{
    public class CrmSaaSDbContext : DbContext
    {
        public CrmSaaSDbContext(DbContextOptions<CrmSaaSDbContext> options)
            : base(options)
        {
        }

        public DbSet<Activity> Activities { get; set; } = null!;
        public DbSet<AppUser> AppUsers { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;
        public DbSet<Contact> Contacts { get; set; } = null!;
        public DbSet<ContactNote> ContactNotes { get; set; } = null!;
        public DbSet<Deal> Deals { get; set; } = null!;
        public DbSet<Lead> Leads { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<Tenant> Tenants { get; set; } = null!;
        public DbSet<TenantSettings> TenantSettings { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // SQL Server doesn't allow multiple cascade paths; default to Restrict to avoid cycles.
            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}