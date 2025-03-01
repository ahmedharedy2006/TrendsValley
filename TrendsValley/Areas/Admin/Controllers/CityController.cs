using BooksMine.Models;
using Microsoft.AspNetCore.Mvc;
using TrendsValley.DataAccess.Data;
using TrendsValley.Models.Models;

namespace TrendsValley.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CityController : Controller
    {
        private readonly AppDbContext _db;
        public CityController(AppDbContext db)
        {
            _db = db;
        }


        public IActionResult Index()
        {
            List<City> objList = _db.cities.ToList();
            return View(objList);
        }


        public IActionResult Upsert(int? id)
        {
            City obj = new();
            if (id == null || id == 0)
            {
                return View(obj);
            }
            obj = _db.cities.FirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(City obj)
        {
            if (obj.Id == null)
            {
                await _db.cities.AddAsync(obj);
            }
            else
            {
                _db.cities.Update(obj);
            }
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        //Remove
        public IActionResult Delete(int id)
        {
            City obj =new();
            obj = _db.cities.FirstOrDefault(u => u.Id == id);
            if(obj == null)
            {
                return NotFound();
            }
            else
            {
                _db.cities.Remove(obj);
            }
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
