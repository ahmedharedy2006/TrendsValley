using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using System.Security.Claims;
using TrendsValley.DataAccess.Data;
using TrendsValley.Models.Models;
using TrendsValley.Models.ViewModels;
using System.Net;
using Microsoft.AspNetCore.Http;
using System.Diagnostics.Metrics;
using System.Numerics;
using System.Globalization;
using System.Security.Cryptography;

namespace TrendsValley.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProfileController : BaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<AppUser> _signInManager;



        public ProfileController(UserManager<AppUser> userManager,
                                  AppDbContext db, IEmailSender emailSender, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _db = db;
            _emailSender = emailSender;
            signInManager = _signInManager;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if user is not authenticated
            }
            var profile = await _db.customers.Where(u => u.Id == userId).Include(u => u.state).Include(c => c.city).FirstOrDefaultAsync();

            if (profile == null)
            {
                return NotFound();
            }
            var model = new ProfileViewModel
            {
                Id = profile.Id,
                FName = profile.Fname,
                LName = profile.Lname,
                Email = profile.Email,
                PhoneNumber = profile.phoneNumber,
                Address = profile.StreetAddress,
                PostalCode = "",
                CurrentCity = profile.city.name,
                CurrentState = profile.state.Name,
                Cities = _db.cities.Select(c => new SelectListItem
                {
                    Text = c.name,
                    Value = c.Id.ToString()
                }),
                States = _db.states.Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id.ToString()
                })
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFirstName(ProfileViewModel model)
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if user is not authenticated
            }

            var user = await _db.customers.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            user.Fname = model.FName;
            _db.customers.Update(user);
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "First Name updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateLastName(ProfileViewModel model)
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if user is not authenticated
            }
            var user = await _db.customers.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            user.Lname = model.LName;
            _db.customers.Update(user);
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Last Name updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEmail(ProfileViewModel model)
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if user is not authenticated
            }
            var user = await _db.customers.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }
            if (user.Email != model.Email)
            {
                user.Email = model.Email;
            }

            _db.customers.Update(user);
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Email updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePhone(ProfileViewModel model)
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if user is not authenticated
            }
            var user = await _db.customers.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }
            user.phoneNumber = model.PhoneNumber;
            _db.customers.Update(user);
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Phone Number updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAddress(ProfileViewModel model)
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if user is not authenticated
            }
            var user = await _db.customers.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            user.StreetAddress = model.Address;
            _db.customers.Update(user);
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Address updated successfully!";
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> UpdateCity(ProfileViewModel model)
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if user is not authenticated
            }
            var user = await _db.customers.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            user.CityId = model.CityId;
            _db.customers.Update(user);
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "City updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateState(ProfileViewModel model)
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if user is not authenticated
            }
            var user = await _db.customers.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            user.StateId = model.stateId;
            _db.customers.Update(user);
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "State updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Security()
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if user is not authenticated
            }
            var user = await _db.customers.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var model = new ProfileViewModel
            {
                Email = user.Email,
                IsTwoFactorEnabled = true,
                ConnectedDevices = await _db.UserDevices
                    .Where(d => d.UserId == user.Id)
                    .OrderByDescending(d => d.LastLoginDate)
                    .Take(2)
                    .ToListAsync(),
                RecentSecurityActivities = await _db.SecurityActivities
                .Where(a => a.UserId == user.Id)
                .OrderByDescending(a => a.ActivityDate)
                .Take(5)
                .ToListAsync()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveDevice(string deviceId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var device = await _db.UserDevices
                .FirstOrDefaultAsync(d => d.Id == Guid.Parse(deviceId) && d.UserId == user.Id);

            if (device == null)
            {
                TempData["Error"] = "The Device Is Not Found";
                return RedirectToAction("ManageDevices");
            }


            await _userManager.UpdateSecurityStampAsync(user);

            _db.UserDevices.Remove(device);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Device removed and logout activated successfully!";
            return RedirectToAction("ManageDevices");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveInactiveDevices()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var thirtyDaysAgo = DateTime.Now.AddDays(-30);
            var inactiveDevices = await _db.UserDevices
                .Where(d => d.UserId == user.Id &&
                           d.LastLoginDate < thirtyDaysAgo &&
                           d.DeviceToken != Request.Cookies["DeviceToken"])
                .ToListAsync();

            if (inactiveDevices.Any())
            {
                await _userManager.UpdateSecurityStampAsync(user);

                _db.UserDevices.RemoveRange(inactiveDevices);
                await _db.SaveChangesAsync();

                TempData["Success"] = $"Removed {inactiveDevices.Count} inactive devices and activated logout!";
            }
            else
            {
                TempData["Info"] = "No inactive devices to remove";
            }

            return RedirectToAction("ManageDevices");
        }

        public IActionResult Payments()
        {
            return View();
        }

        public IActionResult Orders()
        {
            return View();
        }

        public IActionResult ContactUs()
        {
            return View();
        }

    }
}
