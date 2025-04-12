using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TrendsValley.DataAccess.Data;
using TrendsValley.DataAccess.Repository;
using TrendsValley.DataAccess.Repository.Interfaces;
using TrendsValley.Models.Models;
using TrendsValley.Models.ViewModels;

namespace TrendsValley.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<AppUser> _userManager;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment,UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        // GET All Products Method and View
        [Area("Admin")]
        [HttpGet("[area]/[controller]/[action]")]
        [Authorize(Policy = "ViewProduct")]
        public async Task<IActionResult> Index(int pg = 1,string searchTerm = "")
        {
            // Get All Products from Database
            // Include related entities (Product_Brand and Product_Category) using Include method
            var products = await _unitOfWork.ProductRepo.GetAllAsync(

                null,
                new Expression<Func<Product, object>>[]
                {
                    p => p.Product_Brand,
                    p => p.Product_Category
                }

                );

            // Convert the result to a list
            List<Product> objList = products.ToList();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                objList = objList.Where(b =>
                    b.Product_Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
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

            // Return the list to the view
            return View(objList);
        }

        // GET Create and Edit Method and View
        public async Task<IActionResult> Upsert(int? id)
        {
            if (User.HasClaim("Add Product", "Add Product"))
            {
                // Create a new ProductViewModel object
                ProductViewModel obj = new();

                // Get all brands and categories from the database
                var brands = await _unitOfWork.BrandRepo.GetAllAsync();

                // Create a list of SelectListItem for brands
                obj.BrandList = brands.Select(i => new SelectListItem
                {
                    Text = i.Brand_Name,
                    Value = i.Brand_Id.ToString()
                });

                // Get all categories from the database
                var categories = await _unitOfWork.CategoryRepo.GetAllAsync();

                // Create a list of SelectListItem for categories
                obj.CategoryList = categories.Select(i => new SelectListItem
                {
                    Text = i.Category_Name,
                    Value = i.Category_id.ToString()
                });

                // If id is null or 0, return the view with the new object
                if (id == null || id == 0)
                {
                    //Create product
                    return View(obj);
                }
            }

            else if (User.HasClaim("Edit Product", "Edit Product")) {
                // If id is not null or 0, get the Product object from the database
                ProductViewModel obj = new();

                 obj.product = await _unitOfWork.ProductRepo.GetAsync(u => u.Product_Id == id);

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
        public async Task<IActionResult> Upsert(ProductViewModel obj, IFormFile? file)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            string wwwRootPath = _webHostEnvironment.WebRootPath;

            // Check if the model state is valid
            if (file != null)
            {
                if (!string.IsNullOrEmpty(obj.product.imgUrl))
                {
                    string oldImagePath = Path.Combine(wwwRootPath, obj.product.imgUrl.TrimStart('\\', '/'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Generate a unique file name using Guid
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                // Combine the wwwroot path with the desired folder and file name
                string productPath = Path.Combine(wwwRootPath, @"pics");

                using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                {
                    // Copy the uploaded file to the specified path
                    file.CopyTo(fileStream);
                }

                // Set the image URL to the relative path of the uploaded file
                obj.product.imgUrl = @"\pics\" + fileName;
            }

            // Validate the model state
            if (obj.product.Product_Id == 0)
            {
                //Create Product
                await _unitOfWork.ProductRepo.CreateAsync(obj.product);
                await _unitOfWork.ProductRepo.AdminActivityAsync(
                userId: user.Id,
                activityType: "AddProduct",
                description: $"Add Product(Id: {obj.product.Product_Id})",
                ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString()
                );
            }
            else
            {
                //Update Product
               await _unitOfWork.ProductRepo.UpdateAsync(obj.product);
                await _unitOfWork.ProductRepo.AdminActivityAsync(
                userId: user.Id,
                activityType: "UpdateProduct",
                description: $"Update Product(Id: {obj.product.Product_Id})",
                ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString()
                );
            }

            //redirect to index
            return RedirectToAction(nameof(Index));
        }

        // GET Delete Method and View
        [Authorize(Policy = "DeleteProduct")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            // Get the Product object from the database
            Product obj = new();

            // If id is not null or 0, get the Product object from the database
            obj = await _unitOfWork.ProductRepo.GetAsync(u => u.Product_Id == id);

            // If the object is null, return NotFound
            if (obj == null)
            {
                return NotFound();
            }

            string webRootPath = _webHostEnvironment.WebRootPath;

            // Ensure image path is correct by removing the leading slash (if present)
            string imagePath = Path.Combine(webRootPath, obj.imgUrl.TrimStart('\\', '/'));

            // Check if file exists and delete it
            if (System.IO.File.Exists(imagePath)) // Use System.IO.File instead of File
            {
                System.IO.File.Delete(imagePath); 
            }

            // Delete the Product object from the database
            await _unitOfWork.ProductRepo.RemoveAsync(obj);
            await _unitOfWork.ProductRepo.AdminActivityAsync(
            userId: user.Id,
            activityType: "RemoveProduct",
            description: $"Remove Product(Id: {obj.Product_Id})",
            ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString()
            );

            //redirect to index
            return RedirectToAction(nameof(Index));
        }
    }
}
