using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TrendsValley.Models.Models;

namespace TrendsValley.Areas.Customer.Controllers
{
    [Area("Customer")]

    public class SettingController : BaseController
    {

        public SettingController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);

            return View(user); 
        }

        [HttpPost]
        public async Task<IActionResult> ChangeLanguage(string PreferredLanguage)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                user.PreferredLanguage = PreferredLanguage;
                await _userManager.UpdateAsync(user);

                if (HttpContext.Session != null)
                {
                    HttpContext.Session.SetString("lang", PreferredLanguage);
                }
            }

            return RedirectToAction("Index");
        }

    }
}
