using PropertyManagementApp.Models;

namespace PropertyManagementApp.Services
{
    public interface IPropertyService
    {
        Task<List<Property>> GetPropertiesAsync(string landlordId);
        Task<Property?> GetPropertyAsync(int id, string landlordId);
        Task<Property> CreatePropertyAsync(Property property, string landlordId);
        Task<bool> UpdatePropertyAsync(Property property, string landlordId);
        Task<bool> DeletePropertyAsync(int id, string landlordId);
    }
}
