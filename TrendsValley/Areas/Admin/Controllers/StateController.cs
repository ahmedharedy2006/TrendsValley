using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrendsValley.DataAccess.Data;
using TrendsValley.DataAccess.Repository.Interfaces;
using TrendsValley.Models.Models;

namespace TrendsValley.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class StateController : Controller
    {
        private readonly IStateRepo _stateRepoo;
        public StateController(IStateRepo stateRepo)
        {
            _stateRepoo = stateRepo;
        }

        public async Task<IActionResult> Index()
        {
            List<State> objList = await _stateRepoo.GetAllAsync();
            return View(objList);
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            State obj = new();
            if (id == null || id == 0)
            {
                return View(obj);
            }
            obj = await _stateRepoo.GetAsync(u => u.Id == id);
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
                await _stateRepoo.CreateAsync(obj);
            }
            else
            {
                await _stateRepoo.UpdateAsync(obj);
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            State obj = new();
            obj = await _stateRepoo.GetAsync(s => s.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            else
            {
                await _stateRepoo.RemoveAsync(obj);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
