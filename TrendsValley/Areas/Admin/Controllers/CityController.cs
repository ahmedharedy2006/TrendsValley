using TrendsValley.Models.Models;
using Microsoft.AspNetCore.Mvc;
using TrendsValley.DataAccess.Data;
using TrendsValley.DataAccess.Repository.Interfaces;
using TrendsValley.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using TrendsValley.DataAccess.Repository;

namespace TrendsValley.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class CityController : Controller
    {
        private readonly ICityRepo _cityRepo;
        private readonly UserManager<AppUser> _userManager;

        public CityController(ICityRepo cityRepo, UserManager<AppUser> userManager)
        {
            _cityRepo = cityRepo;
            _userManager = userManager;
        }

        // GET All Cities Method and View
        [Authorize(Policy = "ViewCity")]
        public async Task<IActionResult> Index()
        {
            // Get All Cities from Database
            List<City> objList = await _cityRepo.GetAllAsync();

            // Return the list to the view
            return View(objList);
        }

        // GET Create and Edit Method and View
        public async Task<IActionResult> Upsert(int? id)
        {
            if (User.HasClaim("Add City", "Add City")) { 
                // Create a new City object
                City obj = new();

            // If id is null or 0, return the view with the new object
                if (id == null || id == 0)
                {
                    return View(obj);
                }

            }
            else if(User.HasClaim("Edit City", "Edit City")) { 
            // If id is not null or 0, get the City object from the database
            City obj = await _cityRepo.GetAsync(u => u.Id == id);

            // If the object is null, return NotFound
            if (obj == null)
            {
                return NotFound();
            }

            // If the object is not null, return the view with the object
            return View(obj);
            }
            
                return RedirectToAction("Index", "Dashboard");
            

        }

        // POST Create and Edit Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(City obj)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Check if the model state is valid
            if (obj.Id == null)
            {
                // If the object is null or 0, create a new object
                await _cityRepo.CreateAsync(obj);
                await _cityRepo.AdminActivityAsync(
               userId: user.Id,
               activityType: "AddCity",
               description: $"Add City(Id: {obj.Id})",
               ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString()
                );

            }
            else
            {
                // If the object is not null or 0, update the existing object
                await _cityRepo.UpdateAsync(obj);
                await _cityRepo.AdminActivityAsync(
                userId: user.Id,
                activityType: "UpdateCity",
                description: $"Update City(Id: {obj.Id})",
                ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString()
                );
            }

            // Redirect to the Index action
            return RedirectToAction(nameof(Index));
        }

        // GET Delete Method and View
        [Authorize(Policy = "DeleteCity")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            // Get the City object from the database
            City obj =new();

            // If the object is null, return NotFound
            obj = await _cityRepo.GetAsync(u => u.Id == id);

            // If id is not null or 0, get the City object from the database
            if (obj == null)
            {
                return NotFound();
            }
            else
            {
                await _cityRepo.RemoveAsync(obj);
                await _cityRepo.AdminActivityAsync(
                userId: user.Id,
                activityType: "RemoveCity",
                description: $"Remove City(Id: {obj.Id})",
                ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString()
                );
            }

            // Redirect to the Index action
            return RedirectToAction(nameof(Index));
        }
    }
}
