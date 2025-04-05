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

        public async Task<IActionResult> Index()
        {
            List<Brand> objList = await _brandRepo.GetAllAsync(); 
            return View(objList);
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            Brand obj = new();
            if(id == null || id == 0)
            {
                return View(obj);
            }
            obj = await _brandRepo.GetAsync(u => u.Brand_Id == id);
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
                await _brandRepo.CreateAsync(obj);
            }
            else
            {
                _brandRepo.UpdateAsync(obj);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            Brand obj = new();
            obj = await _brandRepo.GetAsync(c => c.Brand_Id == id);

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
