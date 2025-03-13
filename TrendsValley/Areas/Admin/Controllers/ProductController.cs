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
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment; 
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        //View All Product
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
        //Upsert
        public async Task<IActionResult> Upsert(int? id)
        {
            ProductViewModel obj = new();
            //Brand List
            var brands = await _unitOfWork.BrandRepo.GetAllAsync();
            obj.BrandList = brands.Select(i => new SelectListItem
            {
                Text = i.Brand_Name,
                Value = i.Brand_Id.ToString()
            });
            // Category List
            var categories = await _unitOfWork.CategoryRepo.GetAllAsync();

            obj.CategoryList = categories.Select(i => new SelectListItem
            {
                Text = i.Category_Name,
                Value = i.Category_id.ToString()
            });
            if (id == null || id == 0)
            {
                // to Create product
                return View(obj);
            }
            //To Edit

            obj.product = await _unitOfWork.ProductRepo.GetAsync(u => u.Product_Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        //Add and Update Column
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ProductViewModel obj, IFormFile? file)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (file != null)
            {
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

        //Delete
        public async Task<IActionResult> Delete(int id)
        {
            Product obj = new();
            obj = await _unitOfWork.ProductRepo.GetAsync(u => u.Product_Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            await _unitOfWork.ProductRepo.RemoveAsync(obj);
            return RedirectToAction(nameof(Index));
        }
    }
}
