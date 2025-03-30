using Microsoft.AspNetCore.Mvc;

namespace TrendsValley.Areas.Customer.Controllers
{
    [Area("Customer")]

    public class SettingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
