using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Security.Claims;
using TrendsValley.DataAccess.Repository.Interfaces;
using TrendsValley.Models.Models;
using TrendsValley.Models.ViewModels;

namespace TrendsValley.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartViewModel cart = new()
            {
                ListCart = _unitOfWork.ShoppingCartRepo.GetAllAsync(
                    s => s.UserId == userId,
                    new Expression<Func<ShoppingCart, object>>[] { s => s.Product }
                    ).Result.ToList(),

                OrderHeader = new()
            };

            foreach (var item in cart.ListCart)
            {
                cart.OrderHeader.orderTotal += (double)(item.Count * item.Product.Product_Price);
            }


            return View(cart);
        }
    }
}
