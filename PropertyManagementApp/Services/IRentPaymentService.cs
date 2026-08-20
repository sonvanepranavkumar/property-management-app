using PropertyManagementApp.Models;

namespace PropertyManagementApp.Services
{
    public interface IRentPaymentService
    {
        Task<List<RentPayment>> GetRentPaymentsAsync();
        Task<RentPayment?> GetRentPaymentAsync(int id);
        Task<List<RentPayment>> GetRentPaymentsForTenantAsync(int tenantId);
        Task<List<RentPayment>> GetRentPaymentsForPropertyAsync(int propertyId);
        Task<bool> RentPaidAsync(int id);
        Task<bool> CancelRentPaymentAsync(int id);
        Task<RentPayment> CreateRentPaymentAsync(RentPayment rentPayment);
        Task<bool> DeleteRentPaymentAsync(int id);
    }
}
