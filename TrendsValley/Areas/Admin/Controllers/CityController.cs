using TrendsValley.Models.Models;
using Microsoft.AspNetCore.Mvc;
using TrendsValley.DataAccess.Data;
using TrendsValley.DataAccess.Repository.Interfaces;
using TrendsValley.Models.Models;

namespace TrendsValley.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CityController : Controller
    {
        private readonly ICityRepo _cityRepo;
        public CityController(ICityRepo cityRepo)
        {
            _cityRepo = cityRepo;
        }


        public async Task<IActionResult> Index()
        {
            List<City> objList = await _cityRepo.GetAllAsync();
            return View(objList);
        }


        public async Task<IActionResult> Upsert(int? id)
        {
            City obj = new();
            if (id == null || id == 0)
            {
                return View(obj);
            }
            obj = await _cityRepo.GetAsync(u => u.Id == id);
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
                await _cityRepo.CreateAsync(obj);
            }
            else
            {
               await _cityRepo.UpdateAsync(obj);
            }
            return RedirectToAction(nameof(Index));
        }


        //Remove
        public async Task<IActionResult> Delete(int id)
        {
            City obj =new();
            obj = await _cityRepo.GetAsync(u => u.Id == id);
            if(obj == null)
            {
                return NotFound();
            }
            else
            {
                await _cityRepo.RemoveAsync(obj);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
