using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrendsValley.DataAccess.Data;
using TrendsValley.Models.Models;
using TrendsValley.Models.ViewModels;

namespace TrendsValley.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment; 
        public ProductController(AppDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }
        //View All Product
        public IActionResult Index()
        {
            List<Product> objList = _db.Products.Include(u => u.Product_Brand).ToList();
            return View(objList);
        }
        //Upsert
        public IActionResult Upsert(int? id)
        {
            ProductViewModel obj = new();
            //Brand List
            obj.BrandList = _db.Brands.Select(i => new SelectListItem
            {
                Text = i.Brand_Name,
                Value = i.Brand_Id.ToString()
            });
            // Category List
            obj.CategoryList = _db.Categories.Select(i => new SelectListItem
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
            obj.product = _db.Products.FirstOrDefault(u => u.Product_Id == id);
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
                await _db.Products.AddAsync(obj.product);
            }
            else
            {
                //Update Product
                _db.Products.Update(obj.product);
            }
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Delete
        public async Task<IActionResult> Delete(int id)
        {
            Product obj = new();
            obj = await _db.Products.FirstOrDefaultAsync(u => u.Product_Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.Products.Remove(obj);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
