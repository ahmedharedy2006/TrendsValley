using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TrendsValley.DataAccess.Data;
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
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
           var products = await _unitOfWork.ProductRepo.GetAllAsync(

                null,
                new Expression<Func<Product, object>>[]
                {
                    p => p.Product_Brand,
                    p => p.Product_Category
                }

                );

            List<Product> objList = products.ToList();

            return View(objList);
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            ProductViewModel obj = new();
            var brands = await _unitOfWork.BrandRepo.GetAllAsync();
            obj.BrandList = brands.Select(i => new SelectListItem
            {
                Text = i.Brand_Name,
                Value = i.Brand_Id.ToString()
            });
            var categories = await _unitOfWork.CategoryRepo.GetAllAsync();

            obj.CategoryList = categories.Select(i => new SelectListItem
            {
                Text = i.Category_Name,
                Value = i.Category_id.ToString()
            });
            if (id == null || id == 0)
            {
                //Create product
                return View(obj);
            }
            //Edit

            obj.product = await _unitOfWork.ProductRepo.GetAsync(u => u.Product_Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ProductViewModel obj, IFormFile? file)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
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

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string productPath = Path.Combine(wwwRootPath, @"pics");

                using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
                obj.product.imgUrl = @"\pics\" + fileName;
            }
            if (obj.product.Product_Id == 0)
            {
                //Add Product
                await _unitOfWork.ProductRepo.CreateAsync(obj.product);
            }
            else
            {
                //Update Product
               await _unitOfWork.ProductRepo.UpdateAsync(obj.product);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            Product obj = new();
            obj = await _unitOfWork.ProductRepo.GetAsync(u => u.Product_Id == id);
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

            await _unitOfWork.ProductRepo.RemoveAsync(obj);
            return RedirectToAction(nameof(Index));
        }
    }
}
