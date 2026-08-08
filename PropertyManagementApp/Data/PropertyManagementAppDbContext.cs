using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PropertyManagementApp.Models;

namespace PropertyManagementApp.Data
{
    public class PropertyManagementAppDbContext : IdentityDbContext<PropertyManagementAppUser>
    {
        public PropertyManagementAppDbContext(DbContextOptions<PropertyManagementAppDbContext> options) : base(options)
        {
        }

        public DbSet<Property> Properties => Set<Property>();

        public DbSet<Tenant> Tenants => Set<Tenant>();

        public DbSet<RentPayment> RentPayments => Set<RentPayment>();

        public DbSet<Expense> Expenses => Set<Expense>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<RentPayment>()
                .HasOne(r => r.Property)
                .WithMany(p => p.RentPayments)
                .HasForeignKey(r => r.PropertyId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<RentPayment>()
                .HasOne(r => r.Tenant)
                .WithMany(t => t.RentPayments)
                .HasForeignKey(r => r.TenantId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
