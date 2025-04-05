using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using System.Linq.Expressions;
using System.Security.Claims;
using TrendsValley.DataAccess.Data;
using TrendsValley.DataAccess.Repository.Interfaces;
using TrendsValley.Models.Models;
using TrendsValley.Models.ViewModels;
using TrendsValley.Utilities;

namespace TrendsValley.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _db;
        public CartController(IUnitOfWork unitOfWork , AppDbContext db)
        {
            _unitOfWork = unitOfWork;
            _db = db;
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

            if (cart.Count != 0)
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

        public async Task<IActionResult> Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var appuser = await _db.appUsers.Where(u => u.Id == userId).Include(u => u.state).Include(c => c.city).FirstOrDefaultAsync();

            ShoppingCartViewModel cart = new()
            {
                city = appuser.city.name,
                state = appuser.state.Name,
                ListCart = _unitOfWork.ShoppingCartRepo.GetAllAsync(
                    s => s.UserId == userId,
                    new Expression<Func<ShoppingCart, object>>[] { s => s.Product }
                    ).Result.ToList(),
                OrderHeader = new OrderHeader()
                {
                    appUser = appuser,
                },
                CityList = _db.cities.Select(i => new SelectListItem
                {
                    Text = i.name,
                    Value = i.Id.ToString()
                }),
                Statelist = _db.states.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),

            };
            foreach (var item in cart.ListCart)
            {
                cart.OrderHeader.orderTotal += (double)(item.Count * item.Product.Product_Price);
            }
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(ShoppingCartViewModel cart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _unitOfWork.AppUserRepo.GetAsync(u => u.Id == userId);

            cart.ListCart = _unitOfWork.ShoppingCartRepo.GetAllAsync(
                s => s.UserId == userId,
                new Expression<Func<ShoppingCart, object>>[] { s => s.Product }
                ).Result.ToList();
            
            if(cart.OrderHeader.cityId == 0 || cart.OrderHeader.stateId == 0)
            {
                cart.OrderHeader.cityId = user.CityId;
                cart.OrderHeader.stateId = user.StateId;
            }

            cart.OrderHeader.OrderDate = DateTime.Now;
            cart.OrderHeader.AppUserId = userId;
            cart.OrderHeader.appUser = user;
            foreach (var item in cart.ListCart)
            {
                cart.OrderHeader.orderTotal += (double)(item.Count * item.Product.Product_Price);
            }

            cart.OrderHeader.paymentStatus = SD.PaymentStatusPending;
            cart.OrderHeader.orderStatus = SD.StatusPending;

            await _unitOfWork.orderHeaderRepo.CreateAsync(cart.OrderHeader);

            foreach (var item in cart.ListCart)
            {
                item.Product = await _unitOfWork.ProductRepo.GetAsync(p => p.Product_Id == item.ProductId);
                OrderDetails orderDetails = new()
                {
                    productId = item.ProductId,
                    orderHeaderId = cart.OrderHeader.Id,
                    price = (double)item.Product.Product_Price,
                    count = item.Count,
                };
                await _unitOfWork.orderDetailsRepo.CreateAsync(orderDetails);
            }

            var options = new SessionCreateOptions
            {
                SuccessUrl = $"https://localhost:7034/Customer/Cart/orderConfirm?id={cart.OrderHeader.Id}",
                CancelUrl = "https://localhost:7034/Customer/Cart/index",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var item in cart.ListCart)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Product.Product_Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Product_Name,
                        },
                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);

            await _unitOfWork.orderHeaderRepo
                              .UpdateStripePaymentIntentId(cart.OrderHeader.Id,
                                                           session.PaymentIntentId, session.Id);
            await _unitOfWork.SaveAsync();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public async Task<IActionResult> orderConfirm(int id)
        {
            OrderHeader orderHeader = await _unitOfWork.orderHeaderRepo.GetAsync(u => u.Id == id);

            var service = new SessionService();
            Session session = service.Get(orderHeader.sessionId);

            if(session.PaymentStatus.ToLower() == "paid")
            {
                await _unitOfWork.orderHeaderRepo.UpdateStripePaymentIntentId(id, session.PaymentIntentId ,session.Id);
                await _unitOfWork.orderHeaderRepo.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                await _unitOfWork.SaveAsync();
            }
            var carts = await _unitOfWork.ShoppingCartRepo.GetAllAsync(u => u.UserId == orderHeader.AppUserId);

            List<ShoppingCart> shoppingCarts = carts.ToList();

            await _unitOfWork.ShoppingCartRepo.RemoveRangeAsync(shoppingCarts);
            await _unitOfWork.SaveAsync();

            return View(id);
        }
    }
}
