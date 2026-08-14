using Microsoft.EntityFrameworkCore;
using PropertyManagementApp.Data;
using PropertyManagementApp.Models;

namespace PropertyManagementApp.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly PropertyManagementAppDbContext _context;

        public PropertyService(PropertyManagementAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Property>> GetPropertiesAsync()
        {
            return await _context.Properties
                .Include(p => p.Tenants)
                .ToListAsync();
        }

        public async Task<Property?> GetPropertyAsync(int id)
        {
            return await _context.Properties
                .Include(p => p.Tenants)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Property> CreatePropertyAsync(Property property)
        {
            _context.Properties.Add(property);
            await _context.SaveChangesAsync();

            return property;
        }

        public async Task<bool> UpdatePropertyAsync(Property property)
        {
            var existingProperty = await _context.Properties.FindAsync(property.Id);

            if (existingProperty == null)
            {
                return false;
            }

            existingProperty.Address = property.Address;
            existingProperty.Rent = property.Rent;
            existingProperty.Status = property.Status;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeletePropertyAsync(int id)
        {
            var existingProperty = await _context.Properties.FindAsync(id);

            if (existingProperty == null)
            {
                return false;
            }

            _context.Properties.Remove(existingProperty);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
