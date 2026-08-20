using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyManagementApp.Models;
using PropertyManagementApp.Services;
using System.Security.Claims;

namespace PropertyManagementApp.Controllers
{
    [Authorize]
    public class RentPaymentController : Controller
    {
        private readonly IRentPaymentService _rentPaymentService;
        private readonly IPropertyService _propertyService;

        public RentPaymentController(IRentPaymentService rentPaymentService, IPropertyService propertyService)
        {
            _rentPaymentService = rentPaymentService;
            _propertyService = propertyService;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }

        public async Task<IActionResult> Index(int propertyId, string? tenantName)
        {
            var landlordId = GetCurrentUserId();

            var property = await _propertyService.GetPropertyAsync(propertyId, landlordId);

            if (property == null)
            {
                return NotFound();
            }

            var rentPayments = await _rentPaymentService.GetRentPaymentsForPropertyAsync(propertyId);

            if (!string.IsNullOrEmpty(tenantName))
            {
                rentPayments = rentPayments
                    .Where(r => r.TenantName == tenantName)
                    .ToList();
            }

            ViewBag.PropertyId = propertyId;
            ViewBag.Tenants = property.Tenants;
            ViewBag.TenantName = tenantName;

            return View(rentPayments);
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
            ViewBag.Tenants = property.Tenants;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RentPayment rentPayment)
        {
            if (!ModelState.IsValid)
            {
                var landlordId = GetCurrentUserId();

                var property = await _propertyService.GetPropertyAsync(rentPayment.PropertyId, landlordId);

                if (property == null)
                {
                    return NotFound();
                }

                ViewBag.PropertyId = rentPayment.PropertyId;
                ViewBag.Tenants = property.Tenants;

                return View(rentPayment);
            }

            await _rentPaymentService.CreateRentPaymentAsync(rentPayment);

            return RedirectToAction(
                "Index",
                new { propertyId = rentPayment.PropertyId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RentPaid(int id, int propertyId)
        {
            var rentPaid = await _rentPaymentService.RentPaidAsync(id);

            if (!rentPaid)
            {
                return NotFound();
            }

            return RedirectToAction(
                "Index",
                new { propertyId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelRentPayment(int id, int propertyId)
        {
            var cancelledRentPayment = await _rentPaymentService.CancelRentPaymentAsync(id);

            if (!cancelledRentPayment)
            {
                return NotFound();
            }

            return RedirectToAction(
                "Index",
                new { propertyId });
        }
    }
}
