using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrendsValley.DataAccess.Data;
using TrendsValley.Models.Models;
using TrendsValley.Models.ViewModels;
using TrendsValley.Utilities;

namespace TrendsValley.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AuthController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _db;
        public AuthController(SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            UserManager<AppUser> userManager, AppDbContext db)
        {
            _roleManager = roleManager;
            _signInManager = signInManager;
            _userManager = userManager;
            _db = db;
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(LoginViewModel obj)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(obj.Email, obj.Password, obj.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await _userManager.GetUserAsync(User);

                    if (await _userManager.IsInRoleAsync(user, SD.Admin))
                    {
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                    }

                    return RedirectToAction("Index", "Home", new { area = "Customer" });

                }
            }

            return View(obj);
        }

        public IActionResult Register()
        {
            RegisterViewModel obj = new();

            obj.CityList = _db.cities.Select(i => new SelectListItem
            {
                Text = i.name,
                Value = i.Id.ToString()
            });

            obj.Statelist = _db.states.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            return View(obj);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel obj)
        {
          

            var user = new AppUser
            {
                UserName = obj.appUser.Email,
                Email = obj.appUser.Email,
                Fname = obj.appUser.Fname,
                Lname = obj.appUser.Lname,
                CityId = obj.appUser.CityId,
                StateId = obj.appUser.StateId,
                StreetAddress = obj.appUser.StreetAddress,
                PhoneNumber = obj.appUser.PhoneNumber,
            };

            var existingUser = await _userManager.FindByEmailAsync(user.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "Email Already Exists !");
            }

            var result = await _userManager.CreateAsync(user, obj.Password);


            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, SD.User);
                await _signInManager.SignInAsync(user, isPersistent: true);

                return RedirectToAction("Index", "Home");
            }

            obj.CityList = _db.cities.Select(i => new SelectListItem
            {
                Text = i.name,
                Value = i.Id.ToString()
            });

            obj.Statelist = _db.states.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> logOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
