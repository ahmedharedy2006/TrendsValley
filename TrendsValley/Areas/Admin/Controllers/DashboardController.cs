using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TrendsValley.Models;

namespace TrendsValley.Controllers
{
    [Area("Admin")]

    public class DashboardController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

      

        
    }
}
