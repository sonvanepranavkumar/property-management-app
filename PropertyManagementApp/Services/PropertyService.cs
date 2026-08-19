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
        
        public async Task<List<Property>> GetPropertiesAsync(string landlordId)
        {
            return await _context.Properties
                .Include(p => p.Tenants)
                .Where(p => p.LandlordId == landlordId)
                .ToListAsync();
        }

        public async Task<Property?> GetPropertyAsync(int id, string landlordId)
        {
            return await _context.Properties
                .Include(p => p.Tenants)
                .FirstOrDefaultAsync(p =>
                    p.Id == id &&
                    p.LandlordId == landlordId);
        }

        public async Task<Property> CreatePropertyAsync(Property property, string landlordId)
        {
            property.LandlordId = landlordId;

            _context.Properties.Add(property);
            await _context.SaveChangesAsync();

            return property;
        }

        public async Task<bool> UpdatePropertyAsync(Property property, string landlordId)
        {
            var existingProperty = await _context.Properties.FirstOrDefaultAsync(p =>
                p.Id == property.Id &&
                p.LandlordId == landlordId);

            if (existingProperty == null)
            {
                return false;
            }

            existingProperty.Rent = property.Rent;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeletePropertyAsync(int id, string landlordId)
        {
            var existingProperty = await _context.Properties
                .Include(p => p.Tenants)
                .FirstOrDefaultAsync(p =>
                    p.Id == id &&
                    p.LandlordId == landlordId);

            if (existingProperty == null)
            {
                return false;
            }

            _context.Tenants.RemoveRange(existingProperty.Tenants);
            _context.Properties.Remove(existingProperty);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
