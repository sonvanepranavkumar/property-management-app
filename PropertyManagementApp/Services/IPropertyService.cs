using PropertyManagementApp.Models;

namespace PropertyManagementApp.Services
{
    public interface IPropertyService
    {
        Task<List<Property>> GetPropertiesAsync();
        Task<Property?> GetPropertyAsync(int id);
        Task<Property> CreatePropertyAsync(Property property);
        Task<bool> UpdatePropertyAsync(Property property);
        Task<bool> DeletePropertyAsync(int id);
    }
}
