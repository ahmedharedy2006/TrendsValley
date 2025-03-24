using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Security.Claims;
using TrendsValley.DataAccess.Data;
using TrendsValley.DataAccess.Repository.Interfaces;
using TrendsValley.Models;
using TrendsValley.Models.Models;
using TrendsValley.Models.ViewModels;
using static System.Reflection.Metadata.BlobBuilder;

namespace TrendsValley.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _db;
        private readonly IProductRepo _productRepo;
        private readonly IShoppingCartRepo _shoppingCartRepo;

        public HomeController(ILogger<HomeController> logger, IShoppingCartRepo shoppingCartRepo , AppDbContext db , IProductRepo productRepo)
        {
            _logger = logger;
            _db = db;
            _productRepo = productRepo;
            _shoppingCartRepo = shoppingCartRepo;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task <IActionResult> products()
        {
            var products = await _db.Products
                                    .Include(p => p.Product_Brand)
                                    .ToListAsync();

            return View(products);
        }

        public async Task<IActionResult> product_Details(int id)
        {
            var product = await _productRepo.GetAsync(u => u.Product_Id == id , 
                false,
                new Expression<Func<Product, object>>[] {
                    b => b.Product_Category,
                    b => b.Product_Brand
                });

            ProductDetailsViewModel productDetailsViewModel = new ProductDetailsViewModel
            {
                product = product,

                CategoryName = product.Product_Category.Category_Name,

                Brandname = product.Product_Brand.Brand_Name
            };

            return View(productDetailsViewModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> product_Details(ProductDetailsViewModel productDetailsViewModel)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCart shoppingCart = new()
            {
                ProductId = productDetailsViewModel.product.Product_Id,
                UserId = userId
            };
            await _shoppingCartRepo.CreateAsync(shoppingCart);

            return RedirectToAction(nameof(products));

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
