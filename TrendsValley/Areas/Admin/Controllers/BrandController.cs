using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using TrendsValley.DataAccess.Data;
using TrendsValley.Models.Models;

namespace TrendsValley.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController : Controller
    {
        private readonly AppDbContext _db;
        public BrandController(AppDbContext db)
        {
            _db = db;
        }


        public IActionResult Index()
        {
            List<Brand> objList = _db.Brands.ToList(); 
            return View(objList);
        }


        public IActionResult Upsert(int? id)
        {
            Brand obj = new();
            if(id == null || id == 0)
            {
                return View(obj);
            }
            obj = _db.Brands.FirstOrDefault(u => u.Brand_Id == id);
            if(obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Brand obj)
        {
            if (obj.Brand_Id == 0)
            {
                await _db.Brands.AddAsync(obj);
            }
            else
            {
                _db.Brands.Update(obj);
            }
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        
        // remove
        public IActionResult Delete(int id)
        {
            Brand obj = new();
            obj = _db.Brands.FirstOrDefault(c => c.Brand_Id == id);

            if (obj == null)
            {
                return NotFound();
            }
            else
            {
                _db.Brands.Remove(obj);
            }
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
