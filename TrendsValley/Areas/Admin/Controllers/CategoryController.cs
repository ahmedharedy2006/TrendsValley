using Microsoft.AspNetCore.Mvc;
using TrendsValley.DataAccess.Data;
using TrendsValley.Models.Models;

namespace TrendsValley.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _db;
        public CategoryController(AppDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<Category> objList = _db.Categories.ToList();
            return View(objList);
        }
        public IActionResult Upsert(int? id)
        {
            Category obj = new();
            if(id == null || id == 0)
            {
                //Create
                return View(obj);
            }
            //edit
            obj = _db.Categories.FirstOrDefault(u => u.Category_id == id);
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
                await _db.Categories.AddAsync(obj);
            }
            else
            {
                _db.Categories.Update(obj);
            }
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        //Remove
        public IActionResult Delete(int id)
        {
            Category obj = new();
             obj = _db.Categories.FirstOrDefault(c => c.Category_id == id);
            if (obj == null)
            {
                return NotFound();
            }
            else
            {
                _db.Categories.Remove(obj);
            }
             _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
