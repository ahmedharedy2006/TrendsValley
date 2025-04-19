using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using TrendsValley.DataAccess.Data;
using TrendsValley.DataAccess.Repository.Interfaces;
using TrendsValley.Models.Models;

namespace TrendsValley.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController : Controller
    {
        private readonly IBrandRepo _brandRepo;

        private readonly UserManager<AppUser> _userManager;


        public BrandController(IBrandRepo brandRepo, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _brandRepo = brandRepo;
            _userManager = userManager;
        }

        // GET All Brands Method and View

        [Area("Admin")]
        [Authorize(Policy = "ViewBrand")]
        [HttpGet("[area]/[controller]/[action]")]
        public async Task<IActionResult> Index(int pg = 1, string searchTerm = "")
        {
            // Get All Brands from Database
            List<Brand> objList = await _brandRepo.GetAllAsync();

            // Apply search filter if searchTerm is provided
            if (!string.IsNullOrEmpty(searchTerm))
            {
                objList = objList.Where(b =>
                    b.Brand_Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            // Pagination logic
            const int pageSize = 8;
            if (pg < 1) pg = 1;

            int recsCount = objList.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;

            objList = objList.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.count = recsCount;

            return View(objList);
        }

        // GET Create and Edit Method and View
        public async Task<IActionResult> Upsert(int? id)
        {
            if (User.HasClaim("Add Brand", "Add Brand"))
            {
                // Create a new Brand object
                Brand obj = new();

                // If id is null or 0, return the view with the new object

                if (id == null || id == 0)
                {
                    return View(obj);
                }
            }
           

            else if (User.HasClaim("Edit Brand", "Edit Brand"))
            {
                // If id is not null or 0, get the Brand object from the database
                Brand obj = await _brandRepo.GetAsync(u => u.Brand_Id == id);

                // If the object is null, return NotFound
                if (obj == null)
                {
                    return NotFound();
                }

                // If the object is not null, return the view with the object
                return View(obj);
            }
           
                return RedirectToAction("Index", "Dashboard");
            
        }

        // POST Create and Edit Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Brand obj)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();


            
                // Check if the model state is valid
                if (obj.Brand_Id == 0)
                {

                    
                        await _brandRepo.CreateAsync(obj);
                        await _brandRepo.AdminActivityAsync(
                          userId: user.Id,
                          activityType: "AddBrand",
                          description: $"Add Brand (Id: {obj.Brand_Id})",
                          ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString()
                           );
 

                  
                }
            
           
            else
            {
               
                    _brandRepo.UpdateAsync(obj);
                    await _brandRepo.AdminActivityAsync(
                        userId: user.Id,
                        activityType: "UpdateBrand",
                        description: $"Update Brand (Id: {obj.Brand_Id})",
                        ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString()
                    );
            }
            return RedirectToAction(nameof(Index));
        }

        // GET Delete Method and View
        [Authorize(Policy = "DeleteBrand")]
        public async Task<IActionResult> Delete(int id)
        {
            // Get the Brand object from the database
            Brand obj = new();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
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
                await _brandRepo.AdminActivityAsync(
                userId: user.Id,
                activityType: "RemoveBrand",
                description: $"Remove Brand (Id: {obj.Brand_Id})",
                ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString()
                );
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
