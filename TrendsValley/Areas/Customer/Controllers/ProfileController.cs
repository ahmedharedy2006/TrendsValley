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

namespace TrendsValley.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProfileController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly IEmailSender _emailSender;


        public ProfileController(UserManager<AppUser> userManager,
                                  AppDbContext db, IEmailSender emailSender)
        {
            _userManager = userManager;
            _db = db;
            _emailSender = emailSender;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value; // Get the current user's ID


            // Fetch the user's profile
            var profile = await _db.appUsers.Where(u => u.Id == userId).Include(u => u.state).Include(c => c.city).FirstOrDefaultAsync();

            if (profile == null)
            {
                return NotFound();
            }
            // Populate the ProfileViewModel
            var model = new ProfileViewModel
            {
                Id = profile.Id,
                FName = profile.Fname,
                LName = profile.Lname,
                Email = profile.Email,
                PhoneNumber = profile.PhoneNumber,
                Address = profile.StreetAddress,
                PostalCode = profile.PostalCode,
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
        public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user's ID
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }


            if (user.Email != model.Email)
            {
                // Mark the new email as unconfirmed
                user.Email = model.Email;
                user.EmailConfirmed = false; // Set email as unconfirmed
            }


            user.Fname = model.FName;
            user.Lname = model.LName;
            user.StreetAddress = model.Address;
            user.CityId = model.CityId;
            user.PostalCode = model.PostalCode;

            // Update the user's phone number
            user.PhoneNumber = model.PhoneNumber;

            // Save changes to the database
            await _userManager.UpdateAsync(user);
            await _db.SaveChangesAsync();

            // Redirect to the Index action with a success message
            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Security()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new ProfileViewModel
            {
                Email = user.Email,
                IsEmailConfirmed = user.EmailConfirmed
            };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendEmailVerificationCode()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return View("Error");
            }

            var verificationCode = new Random().Next(100000, 999999).ToString();

            TempData["EmailVerificationCode"] = verificationCode;

            try
            {
                await _emailSender.SendEmailAsync(user.Email, "Your Verification Code", $"Your verification code is: {verificationCode}");
                Console.WriteLine("Email sent successfully to: " + user.Email);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email: " + ex.Message);
            }

            return RedirectToAction("VerifyEmailCode", new { userId = user.Id });
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmailCode(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var model = new VerifyEmailCodeViewModel
            {
                UserId = user.Id,
                Email = user.Email 
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyEmailCode(VerifyEmailCodeViewModel model)
        {
            // Basic model validation
            if (!ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user != null) model.Email = user.Email;
                ModelState.AddModelError("Code", "Please enter a valid 6-digit code.");
                return View(model);
            }

            // Get the stored code WITHOUT reading it (to prevent deletion)
            var storedCode = TempData.Peek("EmailVerificationCode")?.ToString();

            if (string.IsNullOrEmpty(storedCode)
                || storedCode != model.Code
                || string.IsNullOrEmpty(model.UserId))
            {
                var invalidCodeUser = await _userManager.FindByIdAsync(model.UserId);
                if (invalidCodeUser != null) model.Email = invalidCodeUser.Email;

                ModelState.AddModelError("Code", "The verification code is invalid or has expired.");
                return View(model);
            }

            // Only remove the code AFTER successful verification
            TempData.Remove("EmailVerificationCode");

            // Update user's email confirmation status
            var verifiedUser = await _userManager.FindByIdAsync(model.UserId);
            if (verifiedUser == null)
            {
                return View("Error");
            }

            verifiedUser.EmailConfirmed = true;
            var result = await _userManager.UpdateAsync(verifiedUser);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Failed to verify email. Please try again.");
                model.Email = verifiedUser.Email;
                return View(model);
            }

            return RedirectToAction("Security", "Profile", new { area = "Customer", message = "Email verified successfully!" });
        }
    }

}
