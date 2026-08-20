using Microsoft.EntityFrameworkCore;
using PropertyManagementApp.Data;
using PropertyManagementApp.Models;

namespace PropertyManagementApp.Services
{
    public class RentPaymentService : IRentPaymentService
    {
        private readonly PropertyManagementAppDbContext _context;

        public RentPaymentService(PropertyManagementAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<RentPayment>> GetRentPaymentsAsync()
        {
            return await _context.RentPayments
                .Include(r => r.Property)
                .OrderByDescending(r => r.DueDate)
                .ToListAsync();
        }

        public async Task<RentPayment?> GetRentPaymentAsync(int id)
        {
            return await _context.RentPayments
                .Include(r => r.Property)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<RentPayment>> GetRentPaymentsForPropertyAsync(int propertyId)
        {
            return await _context.RentPayments
                .Where(r => r.PropertyId == propertyId)
                .OrderByDescending(r => r.DueDate)
                .ToListAsync();
        }

        public async Task<bool> RentPaidAsync(int id)
        {
            var rentPayment = await _context.RentPayments.FindAsync(id);

            if (rentPayment == null)
            {
                return false;
            }

            rentPayment.PaymentDate = DateOnly.FromDateTime(DateTime.Today);
            rentPayment.Status = "Paid";

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CancelRentPaymentAsync(int id)
        {
            var rentPayment = await _context.RentPayments.FindAsync(id);

            if (rentPayment == null)
            {
                return false;
            }

            rentPayment.Status = "Cancelled";

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<RentPayment> CreateRentPaymentAsync(RentPayment rentPayment)
        {
            _context.RentPayments.Add(rentPayment);
            await _context.SaveChangesAsync();

            return rentPayment;
        }

        public async Task<bool> DeleteRentPaymentAsync(int id)
        {
            var existingRentPayment = await _context.RentPayments.FindAsync(id);

            if (existingRentPayment == null)
            {
                return false;
            }

            _context.RentPayments.Remove(existingRentPayment);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
