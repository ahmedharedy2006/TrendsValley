using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using TrendsValley.DataAccess.Data;
using TrendsValley.DataAccess.Repository.Interfaces;
using TrendsValley.Models.Models;

namespace TrendsValley.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BrandController : Controller
    {
        private readonly IBrandRepo _brandRepo;
        public BrandController(IBrandRepo brandRepo)
        {
            _brandRepo = brandRepo;
        }

        // GET All Brands Method and View
        public async Task<IActionResult> Index()
        {
            // Get All Brands from Database
            List<Brand> objList = await _brandRepo.GetAllAsync();

            // Return the list to the view
            return View(objList);
        }

        // GET Create and Edit Method and View
        public async Task<IActionResult> Upsert(int? id)
        {
            // Create a new Brand object
            Brand obj = new();

            // If id is null or 0, return the view with the new object
            if (id == null || id == 0)
            {
                return View(obj);
            }

            // If id is not null or 0, get the Brand object from the database
            obj = await _brandRepo.GetAsync(u => u.Brand_Id == id);

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
        public async Task<IActionResult> Upsert(Brand obj)
        {
            // Check if the model state is valid
            if (obj.Brand_Id == 0)
            {
                await _brandRepo.CreateAsync(obj);
            }
            else
            {
                _brandRepo.UpdateAsync(obj);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET Delete Method and View
        public async Task<IActionResult> Delete(int id)
        {
            // Get the Brand object from the database
            Brand obj = new();

            // If id is not null or 0, get the Brand object from the database
            obj = await _brandRepo.GetAsync(c => c.Brand_Id == id);

            // If the object is null, return NotFound
            if (obj == null)
            {
                return NotFound();
            }
            else
            {
                await _brandRepo.RemoveAsync(obj);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
