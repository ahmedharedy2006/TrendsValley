using Microsoft.AspNetCore.Mvc;
using TrendsValley.DataAccess.Data;
using TrendsValley.Models.Models;

namespace TrendsValley.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StateController : Controller
    {
        private readonly AppDbContext _db;
        public StateController(AppDbContext db)
        {
            _db = db;
        }


        public IActionResult Index()
        {
            List<State> objList = _db.states.ToList();
            return View(objList);
        }


        public IActionResult Upsert(int? id)
        {
            State obj = new();
            if (id == null || id == 0)
            {
                return View(obj);
            }
            obj = _db.states.FirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(State obj)
        {
            if (obj == null)
            {
                await _db.states.AddAsync(obj);
            }
            else
            {
                _db.states.Update(obj);
            }
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            State obj = new();
            obj = _db.states.FirstOrDefault(s => s.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            else
            {
                _db.states.Remove(obj);
            }
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
