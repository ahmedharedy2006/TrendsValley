using Microsoft.AspNetCore.Mvc;

namespace TrendsValley.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
