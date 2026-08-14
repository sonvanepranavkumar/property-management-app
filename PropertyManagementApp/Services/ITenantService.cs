using PropertyManagementApp.Models;

namespace PropertyManagementApp.Services
{
    public interface ITenantService
    {
        Task<List<Tenant>> GetTenantsAsync();
        Task<Tenant?> GetTenantAsync(int id);
        Task<List<Tenant>> GetTenantsForPropertyAsync(int propertyId);
        Task<Tenant> CreateTenantAsync(Tenant tenant);
        Task<bool> UpdateTenantAsync(Tenant tenant);
        Task<bool> DeleteTenantAsync(int id);
    }
}
