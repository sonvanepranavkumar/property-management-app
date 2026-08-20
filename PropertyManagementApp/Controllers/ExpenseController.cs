using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyManagementApp.Models;
using PropertyManagementApp.Services;
using System.Security.Claims;

namespace PropertyManagementApp.Controllers
{
    [Authorize]
    public class ExpenseController : Controller
    {
        private readonly IExpenseService _expenseService;
        private readonly IPropertyService _propertyService;

        public ExpenseController(IExpenseService expenseService, IPropertyService propertyService)
        {
            _expenseService = expenseService;
            _propertyService = propertyService;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }

        public async Task<IActionResult> Index(int propertyId)
        {
            var landlordId = GetCurrentUserId();

            var property = await _propertyService.GetPropertyAsync(propertyId, landlordId);

            if (property == null)
            {
                return NotFound();
            }

            var expenses = await _expenseService.GetExpensesForPropertyAsync(propertyId);

            ViewBag.PropertyId = propertyId;

            return View(expenses);
        }

        public async Task<IActionResult> Create(int propertyId)
        {
            var landlordId = GetCurrentUserId();

            var property = await _propertyService.GetPropertyAsync(propertyId, landlordId);

            if (property == null)
            {
                return NotFound();
            }

            ViewBag.PropertyId = propertyId;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Expense expense)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.PropertyId = expense.PropertyId;

                return View(expense);
            }

            await _expenseService.CreateExpenseAsync(expense);

            return RedirectToAction(
                "Index",
                new { propertyId = expense.PropertyId });
        }
    }
}
