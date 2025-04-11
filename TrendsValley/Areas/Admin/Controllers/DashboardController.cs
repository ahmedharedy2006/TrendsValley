using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TrendsValley.Models;
using TrendsValley.Services;

namespace TrendsValley.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly LocalizationService _localizationService;
        public DashboardController(LocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SetLanguage(string lang)
        {
            // Switch language using the service
            _localizationService.SetLanguage(lang);
            return RedirectToAction("Index");
        }
    }
}
