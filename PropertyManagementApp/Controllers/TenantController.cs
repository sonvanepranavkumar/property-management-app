using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyManagementApp.Models;
using PropertyManagementApp.Services;
using System.Security.Claims;

namespace PropertyManagementApp.Controllers
{
    [Authorize]
    public class TenantController : Controller
    {
        private readonly ITenantService _tenantService;
        private readonly IPropertyService _propertyService;

        public TenantController(ITenantService tenantService, IPropertyService propertyService)
        {
            _tenantService = tenantService;
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

            ViewBag.PropertyId = propertyId;

            return View(property.Tenants);
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
        public async Task<IActionResult> Create(int propertyId, Tenant tenant)
        {
            var landlordId = GetCurrentUserId();

            var property = await _propertyService.GetPropertyAsync(propertyId, landlordId);

            if (property == null)
            {
                return NotFound();
            }

            if (tenant.LeaseEndDate.HasValue && tenant.LeaseEndDate < tenant.LeaseStartDate)
            {
                ModelState.AddModelError(
                    "LeaseEndDate",
                    "Lease end date cannot be before the lease start date.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.PropertyId = propertyId;
                return View(tenant);
            }

            tenant.PropertyId = propertyId;

            await _tenantService.CreateTenantAsync(tenant);

            return RedirectToAction(
                "ManageTenants",
                "Property",
                new { id = propertyId });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var tenant = await _tenantService.GetTenantAsync(id);

            if (tenant == null)
            {
                return NotFound();
            }

            return View(tenant);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Tenant tenant)
        {
            if (tenant.LeaseEndDate.HasValue && tenant.LeaseEndDate < tenant.LeaseStartDate)
            {
                ModelState.AddModelError(
                    "LeaseEndDate",
                    "Lease end date cannot be before the lease start date.");
            }

            if (!ModelState.IsValid)
            {
                return View(tenant);
            }

            var updatedTenant = await _tenantService.UpdateTenantAsync(tenant);

            if (!updatedTenant)
            {
                return NotFound();
            }

            return RedirectToAction(
                "ManageTenants",
                "Property",
                new { id = tenant.PropertyId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var tenant = await _tenantService.GetTenantAsync(id);

            if (tenant == null)
            {
                return NotFound();
            }

            var propertyId = tenant.PropertyId;

            await _tenantService.DeleteTenantAsync(id);

            return RedirectToAction(
                "ManageTenants",
                "Property",
                new { id = propertyId });
        }
    }
}
