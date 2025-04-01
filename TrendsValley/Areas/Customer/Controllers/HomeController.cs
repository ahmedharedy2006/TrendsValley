using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Security.Claims;
using TrendsValley.Areas.Customer.Controllers;
using TrendsValley.DataAccess.Data;
using TrendsValley.DataAccess.Repository.Interfaces;
using TrendsValley.Models;
using TrendsValley.Models.Models;
using TrendsValley.Models.ViewModels;
using static System.Reflection.Metadata.BlobBuilder;

namespace TrendsValley.Controllers
{
    [Area("Customer")]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _db;
        private readonly IProductRepo _productRepo;
        private readonly IShoppingCartRepo _shoppingCartRepo;

        public HomeController(ILogger<HomeController> logger, IShoppingCartRepo shoppingCartRepo, AppDbContext db, IProductRepo productRepo)
        {
            _logger = logger;
            _db = db;
            _productRepo = productRepo;
            _shoppingCartRepo = shoppingCartRepo;
        }
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                ViewBag.CartCount = _shoppingCartRepo.GetAllAsync(u => u.UserId == userId).Result.Count();
            }
            else
            {
                ViewBag.CartCount = 0;
            }
            return View();
        }

        public async Task<IActionResult> products(int pg = 1)
        {
            if (User.Identity.IsAuthenticated)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                ViewBag.CartCount = _shoppingCartRepo.GetAllAsync(u => u.UserId == userId).Result.Count();
            }
            else
            {
                ViewBag.CartCount = 0;
            }
            var products = await _db.Products
                                    .Include(p => p.Product_Brand)
                                    .ToListAsync();

            const int pageSize = 8;
            if(pg < 1)
               pg = 1;
            
            int recsCount = products.Count();

            var pager = new Pager(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            products = products.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;

            return View(products);
        }

        public async Task<IActionResult> product_Details(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                ViewBag.CartCount = _shoppingCartRepo.GetAllAsync(u => u.UserId == userId).Result.Count();
            }
            else
            {
                ViewBag.CartCount = 0;
            }
            var product = await _productRepo.GetAsync(u => u.Product_Id == id,
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

            ShoppingCart cart = new ShoppingCart()
            {

                ProductDetailsViewModel = productDetailsViewModel,
                Count = 0,
                ProductId = product.Product_Id,

            };

            return View(cart);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> product_Details(ShoppingCart cart)
        {


            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var AppuserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
           
            ShoppingCart cartFromDB = await _shoppingCartRepo
                                            .GetAsync(u => u.ProductId == cart.ProductId && u.UserId == AppuserId);

            if (cartFromDB != null)
            {
                var newCount = cartFromDB.Count + cart.Count;
                cart.Count = newCount;
                await _shoppingCartRepo.UpdateAsync(cartFromDB);
            }

            else
            {
                ShoppingCart newCart = new()
                {
                    UserId = AppuserId,
                    ProductId = cart.ProductId,
                    Count = cart.Count
                };

                await _shoppingCartRepo.CreateAsync(newCart);
            }

            return RedirectToAction(nameof(products));

        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public ActionResult Search(string searchTerm)
        {
            var viewModel = new SearchViewModel
            {
                SearchTerm = searchTerm
            };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                viewModel.Results = _db.Products
                    .Where(p => p.Product_Name.Contains(searchTerm) ||
                                p.Product_Details.Contains(searchTerm))
                    .ToList();
            }
            else
            {
                viewModel.Results = new List<Product>();
            }

            return View(viewModel);
        }
    }
}
