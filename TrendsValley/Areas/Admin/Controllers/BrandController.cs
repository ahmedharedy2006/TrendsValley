using Microsoft.AspNetCore.Mvc;

namespace TrendsValley.Areas.Admin.Controllers
{
    public class BrandController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
