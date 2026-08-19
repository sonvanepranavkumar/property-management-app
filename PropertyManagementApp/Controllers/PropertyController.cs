using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyManagementApp.Models;
using PropertyManagementApp.Services;
using System.Security.Claims;

namespace PropertyManagementApp.Controllers
{
    [Authorize]
    public class PropertyController : Controller
    {
        private readonly IPropertyService _propertyService;

        public PropertyController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }

        public async Task<IActionResult> Index()
        {
            var landlordId = GetCurrentUserId();
            var properties = await _propertyService.GetPropertiesAsync(landlordId);

            return View(properties);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Property property)
        {
            if (!ModelState.IsValid)
            {
                return View(property);
            }

            var landlordId = GetCurrentUserId();
            await _propertyService.CreatePropertyAsync(property, landlordId);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ManageTenants(int id)
        {
            var landlordId = GetCurrentUserId();

            var property = await _propertyService.GetPropertyAsync(id, landlordId);

            if (property == null)
            {
                return NotFound();
            }

            return View(property);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var landlordId = GetCurrentUserId();

            var property = await _propertyService.GetPropertyAsync(id, landlordId);

            if (property == null)
            {
                return NotFound();
            }

            return View(property);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Property property)
        {
            if (!ModelState.IsValid)
            {
                return View(property);
            }

            var landlordId = GetCurrentUserId();

            var updatedProperty = await _propertyService.UpdatePropertyAsync(property, landlordId);

            if (!updatedProperty)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var landlordId = GetCurrentUserId();

            var deletedProperty = await _propertyService.DeletePropertyAsync(id, landlordId);

            if (!deletedProperty)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
