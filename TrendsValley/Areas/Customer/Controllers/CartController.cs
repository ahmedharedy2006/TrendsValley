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
    public class CartController : BaseController
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

        public async Task<IActionResult> increaseQuantity(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var cart = await _unitOfWork.ShoppingCartRepo
                            .GetAsync(c => c.Id == id && c.UserId == userId, true,
                                new Expression<Func<ShoppingCart, object>>[] { c => c.Product }
                                );

            if (cart.Count < cart.Product.NoInStock)
            {
                cart.Count += 1;
                await _unitOfWork.SaveAsync();

            }
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> decreaseQuantity(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var cart = await _unitOfWork.ShoppingCartRepo
                .GetAsync(c => c.Id == id && c.UserId == userId, true,
                    new Expression<Func<ShoppingCart, object>>[] { c => c.Product }
                );

            if (cart.Count != 0 )
            {
                cart.Count -= 1;
                await _unitOfWork.SaveAsync();

            }

            if (cart.Count == 0)
            {
                await _unitOfWork.ShoppingCartRepo.RemoveAsync(cart);
            }

            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> remove(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var cart = await _unitOfWork.ShoppingCartRepo
                .GetAsync(c => c.Id == id && c.UserId == userId, true,
                    new Expression<Func<ShoppingCart, object>>[] { c => c.Product }
                );

                await _unitOfWork.ShoppingCartRepo.RemoveAsync(cart);
            

            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> ClearCart()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var cart = await _unitOfWork.ShoppingCartRepo.GetAllAsync(u => u.UserId == userId);

            await _unitOfWork.ShoppingCartRepo.RemoveRangeAsync(cart);

            return RedirectToAction(nameof(Index));
        }
    }
}
