using PropertyManagementApp.Models;

namespace PropertyManagementApp.Services
{
    public interface IRentPaymentService
    {
        Task<List<RentPayment>> GetRentPaymentsAsync();
        Task<RentPayment?> GetRentPaymentAsync(int id);
        Task<List<RentPayment>> GetRentPaymentsForTenantAsync(int tenantId);
        Task<List<RentPayment>> GetRentPaymentsForPropertyAsync(int propertyId);
        Task<RentPayment> CreateRentPaymentAsync(RentPayment rentPayment);
        Task<bool> DeleteRentPaymentAsync(int id);
    }
}
