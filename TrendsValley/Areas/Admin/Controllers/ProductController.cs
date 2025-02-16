using Microsoft.AspNetCore.Mvc;

namespace TrendsValley.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
