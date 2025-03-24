using Microsoft.AspNetCore.Identity;
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


        public ProfileController(UserManager<AppUser> userManager,
                                  AppDbContext db)
        {
            _userManager = userManager;
            _db = db;
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
    }
}
