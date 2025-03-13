using Microsoft.AspNetCore.Mvc;
using TrendsValley.DataAccess.Data;
using TrendsValley.DataAccess.Repository.Interfaces;
using TrendsValley.Models.Models;

namespace TrendsValley.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepo _categoryRepo;
        public CategoryController(ICategoryRepo categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }
        public async Task<IActionResult> Index()
        {
            List<Category> objList = await _categoryRepo.GetAllAsync();
            return View(objList);
        }
        public async Task<IActionResult> Upsert(int? id)
        {
            Category obj = new();
            if(id == null || id == 0)
            {
                //Create
                return View(obj);
            }
            //edit
            obj = await _categoryRepo.GetAsync(u => u.Category_id == id);
            if(obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Category obj)
        {
            if (obj.Category_id == 0)
            {
                await _categoryRepo.CreateAsync(obj);
                TempData["Success"] = "Category Added Successfully";

            }
            else
            {
                await _categoryRepo.UpdateAsync(obj);
                TempData["Success"] = "Category Updated Successfully";

            }
            return RedirectToAction(nameof(Index));
        }
        //Remove
        public async Task<IActionResult> Delete(int id)
        {
            Category obj = new();
             obj = await _categoryRepo.GetAsync(c => c.Category_id == id);
            if (obj == null)
            {
                return NotFound();
            }
            else
            {
                await _categoryRepo.RemoveAsync(obj);
                TempData["Success"] = "Category Removed Successfully";

            }
            return RedirectToAction(nameof(Index));
        }
    }
}
