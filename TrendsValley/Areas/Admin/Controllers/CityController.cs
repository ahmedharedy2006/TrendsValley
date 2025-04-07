using TrendsValley.Models.Models;
using Microsoft.AspNetCore.Mvc;
using TrendsValley.DataAccess.Data;
using TrendsValley.DataAccess.Repository.Interfaces;
using TrendsValley.Models.Models;
using Microsoft.AspNetCore.Authorization;

namespace TrendsValley.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class CityController : Controller
    {
        private readonly ICityRepo _cityRepo;
        public CityController(ICityRepo cityRepo)
        {
            _cityRepo = cityRepo;
        }

        // GET All Cities Method and View
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
            // Create a new City object
            City obj = new();

            // If id is null or 0, return the view with the new object
            if (id == null || id == 0)
            {
                return View(obj);
            }

            // If id is not null or 0, get the City object from the database
            obj = await _cityRepo.GetAsync(u => u.Id == id);

            // If the object is null, return NotFound
            if (obj == null)
            {
                return NotFound();
            }

            // If the object is not null, return the view with the object
            return View(obj);
        }

        // POST Create and Edit Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(City obj)
        {
            // Check if the model state is valid
            if (obj.Id == null)
            {
                // If the object is null or 0, create a new object
                await _cityRepo.CreateAsync(obj);
            }
            else
            {
                // If the object is not null or 0, update the existing object
                await _cityRepo.UpdateAsync(obj);
            }

            // Redirect to the Index action
            return RedirectToAction(nameof(Index));
        }

        // GET Delete Method and View
        public async Task<IActionResult> Delete(int id)
        {
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
            }

            // Redirect to the Index action
            return RedirectToAction(nameof(Index));
        }
    }
}
