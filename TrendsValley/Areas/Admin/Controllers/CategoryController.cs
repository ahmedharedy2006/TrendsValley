using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TrendsValley.DataAccess.Data;
using TrendsValley.DataAccess.Repository;
using TrendsValley.DataAccess.Repository.Interfaces;
using TrendsValley.Models.Models;
using TrendsValley.Services;

namespace TrendsValley.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class CategoryController : Controller
    {
        private readonly ICategoryRepo _categoryRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly LocalizationService _localizationService;
        public CategoryController(ICategoryRepo categoryRepo, UserManager<AppUser> userManager , LocalizationService localizationService)
        {
            _categoryRepo = categoryRepo;
            _userManager = userManager;
            _localizationService = localizationService;
        }

        // GET All Categories Method and View
        [Area("Admin")]
        [Authorize(Policy = "ViewCategory")]
        [HttpGet("[area]/[controller]/[action]")]
        public async Task<IActionResult> Index(int pg = 1, string searchTerm = "")
        {
            // Get All Categories from Database
            List<Category> objList = await _categoryRepo.GetAllAsync();

            // Apply search filter
            if (!string.IsNullOrEmpty(searchTerm))
            {
                objList = objList.Where(c =>
                    c.Category_Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
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

            var lang = _localizationService.GetCurrentLanguage();
            _localizationService.Load(lang);

            return View(objList);
        }
        // GET Create and Edit Method and View
        public async Task<IActionResult> Upsert(int? id)
        {
            if (User.HasClaim("Add Category", "Add Category"))
            {
                // Create a new Category object
                Category obj = new();

                // If id is null or 0, return the view with the new object
                if (id == null || id == 0)
                {
                    // If the object is null
                    return View(obj);
                }

            }

            else if (User.HasClaim("Edit Category", "Edit Category"))
            {
                // If id is not null or 0, get the Category object from the database
                Category obj = await _categoryRepo.GetAsync(u => u.Category_id == id);

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
        public async Task<IActionResult> Upsert(Category obj)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            // Check if the model state is valid
            if (obj.Category_id == 0)
            {
                // If the object is null, create a new Category
                await _categoryRepo.CreateAsync(obj);
                await _categoryRepo.AdminActivityAsync(
                       userId: user.Id,
                       activityType: "AddCategory",
                       description: $"Add Category(Id: {obj.Category_id})",
                       ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString()
                   );
                // Set a success message in TempData
                TempData["Success"] = "Category Added Successfully";

            }
            else
            {
                // If the object is not null, update the Category
                await _categoryRepo.UpdateAsync(obj);
                await _categoryRepo.AdminActivityAsync(
                userId: user.Id,
                activityType: "UpdateCategory",
                description: $"Update Category (Id: {obj.Category_id})",
                ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString()
            );
                // Set a success message in TempData
                TempData["Success"] = "Category Updated Successfully";

            }

            // Redirect to the Index action
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Policy = "DeleteCategory")]
        // GET Delete Method and View
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            // Get the Category object from the database
            Category obj = new();

            // If id is not null or 0, get the Category object from the database
            obj = await _categoryRepo.GetAsync(c => c.Category_id == id);

            // If the object is null, return NotFound
            if (obj == null)
            {
                return NotFound();
            }
            else
            {
                await _categoryRepo.RemoveAsync(obj);
                await _categoryRepo.AdminActivityAsync(
                userId: user.Id,
                activityType: "RemoveCategory",
                description: $"Remove Category (Id: {obj.Category_id})",
                ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString()
                );
                TempData["Success"] = "Category Removed Successfully";
            }

            // Redirect to the Index action
            return RedirectToAction(nameof(Index));
        }
    }
}
