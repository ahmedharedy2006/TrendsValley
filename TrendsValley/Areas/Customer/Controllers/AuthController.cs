using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TrendsValley.Models.Models;

namespace TrendsValley.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AuthController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        public AuthController(SignInManager<AppUser> signInManager,RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        
        public IActionResult SignIn()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }
    }
}
