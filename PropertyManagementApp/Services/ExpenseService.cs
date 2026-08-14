using Microsoft.EntityFrameworkCore;
using PropertyManagementApp.Data;
using PropertyManagementApp.Models;

namespace PropertyManagementApp.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly PropertyManagementAppDbContext _context;

        public ExpenseService(PropertyManagementAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Expense>> GetExpensesAsync()
        {
            return await _context.Expenses
                .Include(e => e.Property)
                .OrderByDescending(e => e.ExpenseDate)
                .ToListAsync();
        }

        public async Task<Expense?> GetExpenseAsync(int id)
        {
            return await _context.Expenses
                .Include(e => e.Property)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<Expense>> GetExpensesForPropertyAsync(int propertyId)
        {
            return await _context.Expenses
                .Where(e => e.PropertyId == propertyId)
                .OrderByDescending(e => e.ExpenseDate)
                .ToListAsync();
        }

        public async Task<Expense> CreateExpenseAsync(Expense expense)
        {
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            return expense;
        }

        public async Task<bool> UpdateExpenseAsync(Expense expense)
        {
            var existingExpense = await _context.Expenses.FindAsync(expense.Id);

            if (existingExpense == null)
            {
                return false;
            }

            existingExpense.Category = expense.Category;
            existingExpense.Description = expense.Description;
            existingExpense.Amount = expense.Amount;
            existingExpense.ExpenseDate = expense.ExpenseDate;
            existingExpense.PropertyId = expense.PropertyId;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteExpenseAsync(int id)
        {
            var existingExpense = await _context.Expenses.FindAsync(id);

            if (existingExpense == null)
            {
                return false;
            }

            _context.Expenses.Remove(existingExpense);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
