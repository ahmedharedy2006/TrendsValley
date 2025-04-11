using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Net;
using System.Numerics;
using System.Security.Claims;
using System.Text.Json;
using TrendsValley.DataAccess.Data;
using TrendsValley.Models.Models;
using TrendsValley.Models.ViewModels;
using TrendsValley.Services;
using TrendsValley.Utilities;

namespace TrendsValley.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AuthController : BaseController
    {

        private readonly UserService _userService;
        private readonly AppDbContext _db;

        public AuthController(UserService userService , AppDbContext db)
        {
            _userService = userService;
            _db = db;
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn(LoginViewModel obj)
        {
            if (!ModelState.IsValid) return View(obj);

            var user = await _userService.GetUserByUsername(obj.Email);

            var isValid = await _userService.ValidateUser(user.Username, obj.Password);
            if (user == null || isValid == false)
            {
                ModelState.AddModelError("", "Invalid login attempt");
                return View(obj);
            }

            var claims = new List<Claim>
            {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("Fname", user.Fname),
            new Claim("Lname", user.Lname),
            new Claim(ClaimTypes.NameIdentifier, user.Id), // or use user ID
            new Claim(ClaimTypes.Role, SD.User) // Adding role as a claim

            };

            HttpContext.Session.SetString("UserId", user.Id.ToString()); // Store user ID
            HttpContext.Session.SetString("UserName", user.Username);  // Store username            

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            CustomerRegisterViewModel obj = new();

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
        public async Task<IActionResult> Register(CustomerRegisterViewModel obj)
        {


            var user = new Models.Models.Customer
            {
                Username = obj.appUser.Email,
                Email = obj.appUser.Email,
                Password = obj.Password,
                Fname = obj.appUser.Fname,
                Lname = obj.appUser.Lname,
                CityId = obj.appUser.CityId,
                StateId = obj.appUser.StateId,
                StreetAddress = obj.appUser.StreetAddress,
                phoneNumber = obj.appUser.phoneNumber,
            };

            if (await _userService.UserExists(obj.appUser.Email))
            {
                ViewBag.Message = "Username already exists!";
                return View();
            }

            var success = await _userService.RegisterUser(user , SD.User);
            if (success)
            {
                return RedirectToAction(nameof(SignIn));
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
            HttpContext.Session.Clear();
            return RedirectToAction("SignIn");
        }
    }
}
