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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user's ID
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            // Fetch the user's profile
            var profile = await _db.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);

            // If no profile exists, create a new one with default values
            if (profile == null)
            {
                profile = new Profile
                {
                    UserId = userId,
                      Name = $"{user.Fname} {user.Lname}",
                    Address = "Not Provided", // Default address
                    PostalCode = "Not Provided", // Default postal code
                    Gender = "Not Specified", // Default gender
                    ProfileImageUrl = "/images/default-profile.png" // Default profile image
                };

                // Add the new profile to the database
                _db.Profiles.Add(profile);
                await _db.SaveChangesAsync();
            }

            // Populate the ProfileViewModel
            var model = new ProfileViewModel
            {
                Id = profile.Id,
                Name = user.Fname,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = profile.Address,
                PostalCode = profile.PostalCode,
                Gender = profile.Gender,
                ProfileImageUrl = profile.ProfileImageUrl,
                User = user // Pass the AppUser object to the view
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

            // Fetch the user's profile
            var profile = await _db.Profiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
            {
                // If no profile exists, create a new one
                profile = new Profile { UserId = userId };
                _db.Profiles.Add(profile);
            }

            if (user.Email != model.Email)
            {
                // Mark the new email as unconfirmed
                user.Email = model.Email;
                user.EmailConfirmed = false; // Set email as unconfirmed
            }


            // Update the profile with the model data
            profile.Name = model.Name;
            profile.Address = model.Address;
            profile.CurrentCity = model.CurrentCity;
            profile.PostalCode = model.PostalCode;
            profile.Gender = model.Gender;

            // Update the user's phone number
            user.PhoneNumber = model.PhoneNumber;

            // Save changes to the database
            await _userManager.UpdateAsync(user);
            await _db.SaveChangesAsync();

            // Redirect to the Index action with a success message
            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Profile/UpdateProfileImage
        [HttpPost]
        public async Task<IActionResult> UpdateProfileImage(IFormFile file)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profile = await _db.Profiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
            {
                return NotFound();
            }

            if (file != null && file.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "Profile_Img");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                profile.ProfileImageUrl = "/Profile_Img/" + uniqueFileName;
                _db.Profiles.Update(profile);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
