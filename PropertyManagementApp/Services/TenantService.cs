using Microsoft.EntityFrameworkCore;
using PropertyManagementApp.Data;
using PropertyManagementApp.Models;

namespace PropertyManagementApp.Services
{
    public class TenantService : ITenantService
    {
        private readonly PropertyManagementAppDbContext _context;

        public TenantService(PropertyManagementAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Tenant>> GetTenantsAsync()
        {
            return await _context.Tenants
                .Include(t => t.Property)
                .ToListAsync();
        }

        public async Task<Tenant?> GetTenantAsync(int id)
        {
            return await _context.Tenants
                .Include(t => t.Property)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Tenant>> GetTenantsForPropertyAsync(int propertyId)
        {
            return await _context.Tenants
                .Where(t => t.PropertyId == propertyId)
                .ToListAsync();
        }

        public async Task<Tenant> CreateTenantAsync(Tenant tenant)
        {
            _context.Tenants.Add(tenant);

            var property = await _context.Properties.FirstOrDefaultAsync(p => p.Id == tenant.PropertyId);

            if (property != null)
            {
                property.Status = "Occupied";
            }

            await _context.SaveChangesAsync();

            return tenant;
        }

        public async Task<bool> UpdateTenantAsync(Tenant tenant)
        {
            var existingTenant = await _context.Tenants.FindAsync(tenant.Id);

            if (existingTenant == null)
            {
                return false;
            }

            existingTenant.FirstName = tenant.FirstName;
            existingTenant.LastName = tenant.LastName;
            existingTenant.Email = tenant.Email;
            existingTenant.Phone = tenant.Phone;
            existingTenant.LeaseStartDate = tenant.LeaseStartDate;
            existingTenant.LeaseEndDate = tenant.LeaseEndDate;
            existingTenant.PropertyId = tenant.PropertyId;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteTenantAsync(int id)
        {
            var existingTenant = await _context.Tenants.FindAsync(id);

            if (existingTenant == null)
            {
                return false;
            }

            _context.Tenants.Remove(existingTenant);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
