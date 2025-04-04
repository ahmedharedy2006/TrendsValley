﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TrendsValley.DataAccess.Data;
using TrendsValley.Models.Models;

namespace TrendsValley.Areas.Customer.Controllers
{
    [Area("Customer")]

    public class SettingController : BaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _db;

        public SettingController(UserManager<AppUser> userManager, AppDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeLanguage(string PreferredLanguage)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);


            try
            {
                if (user != null)
                {
                    user.PreferredLanguage = PreferredLanguage;
                    await _userManager.UpdateAsync(user);

                    if (HttpContext.Session != null)
                    {
                        HttpContext.Session.SetString("lang", PreferredLanguage);
                    }
                }

                TempData["SuccessMessage"] = "Language changed successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to change language";
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public async Task<IActionResult> ChangeCurrency(string Currency)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);

            try
            {
                if (user != null)
                {
                    user.Currency = Currency;
                    await _userManager.UpdateAsync(user);
                }
                TempData["SuccessMessage"] = "Currency changed successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to change currency";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangePayment(string PaymentMethod)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            try
            {
                if (user != null)
                {
                    user.PaymentMehtod = PaymentMethod;
                    await _userManager.UpdateAsync(user);
                    TempData["SuccessMessage"] = "Payment method updated successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "User not found";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to update payment method";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ChangePreferredCarriers(string PreferredCarriers)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            try
            {
                if (user != null)
                {
                    user.PreferredCarriers = PreferredCarriers;
                    await _userManager.UpdateAsync(user);
                    TempData["SuccessMessage"] = "Preferred Carriers updated successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "User not found";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to update Preferred Carriers";
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePrimaryAddress(string userId, string addressValue)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found";
                    return RedirectToAction(nameof(Index));
                }

                user.StreetAddress = addressValue;
                user.SelectedAddress = "primary";

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Primary address updated successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update primary address";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating address: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSecondaryAddress(string userId, string addressValue)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found";
                    return RedirectToAction(nameof(Index));
                }

                user.StreetAddress2 = addressValue;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Secondary address updated successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update secondary address";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating address: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPrimaryAddress(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found";
                    return RedirectToAction(nameof(Index));
                }

                if (!string.IsNullOrEmpty(user.StreetAddress2))
                {
                    // Swap addresses
                    var temp = user.StreetAddress;
                    user.StreetAddress = user.StreetAddress2;
                    user.StreetAddress2 = temp;
                    user.SelectedAddress = "primary";

                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        TempData["SuccessMessage"] = "Primary address changed successfully";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Failed to update primary address";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating address: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSecondaryAddress(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found";
                    return RedirectToAction(nameof(Index));
                }

                user.StreetAddress2 = null;

                // If secondary was selected, revert to primary
                if (user.SelectedAddress == "secondary")
                {
                    user.SelectedAddress = "primary";
                }

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Secondary address removed";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to remove address";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error removing address: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Shipping()
        {
            return View();
        }
        public IActionResult Notifications()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
