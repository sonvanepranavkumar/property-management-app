using Microsoft.AspNetCore.Mvc;
using PropertyManagementApp.Services;

namespace PropertyManagementApp.Controllers
{
    public class PropertyController : Controller
    {
        private readonly IPropertyService _propertyService;

        public PropertyController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        public async Task<IActionResult> Index()
        {
            var properties = await _propertyService.GetPropertiesAsync();

            return View(properties);
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}
