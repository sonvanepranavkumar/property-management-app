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
    }
}
