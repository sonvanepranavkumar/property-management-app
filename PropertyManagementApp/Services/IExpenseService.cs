using PropertyManagementApp.Models;

namespace PropertyManagementApp.Services
{
    public interface IExpenseService
    {
        Task<List<Expense>> GetExpensesAsync();
        Task<Expense?> GetExpenseAsync(int id);
        Task<List<Expense>> GetExpensesForPropertyAsync(int propertyId);
        Task<Expense> CreateExpenseAsync(Expense expense);
        Task<bool> UpdateExpenseAsync(Expense expense);
        Task<bool> DeleteExpenseAsync(int id);
    }
}
