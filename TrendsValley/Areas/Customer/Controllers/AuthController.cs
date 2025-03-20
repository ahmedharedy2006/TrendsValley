using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using TrendsValley.DataAccess.Data;
using TrendsValley.Models.Models;
using TrendsValley.Models.ViewModels;
using TrendsValley.Utilities;

namespace TrendsValley.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AuthController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _db;
        private readonly IEmailSender _emailSender;

        public AuthController(SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            UserManager<AppUser> userManager, AppDbContext db ,IEmailSender emailSender)
        {
            _roleManager = roleManager;
            _signInManager = signInManager;
            _userManager = userManager;
            _db = db;
            _emailSender = emailSender;
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(LoginViewModel obj)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(obj.Email, obj.Password, obj.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await _userManager.GetUserAsync(User);

                    if (await _userManager.IsInRoleAsync(user, SD.Admin))
                    {
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                    }

                    return RedirectToAction("Index", "Home", new { area = "Customer" });

                }
            }

            return View(obj);
        }

        public IActionResult Register()
        {
            RegisterViewModel obj = new();

            obj.CityList = _db.cities.Select(i => new SelectListItem
            {
                Text = i.name,
                Value = i.Id.ToString()
            });

            obj.Statelist = _db.states.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            return View(obj);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel obj)
        {
          

            var user = new AppUser
            {
                UserName = obj.appUser.Email,
                Email = obj.appUser.Email,
                Fname = obj.appUser.Fname,
                Lname = obj.appUser.Lname,
                CityId = obj.appUser.CityId,
                StateId = obj.appUser.StateId,
                StreetAddress = obj.appUser.StreetAddress,
                PhoneNumber = obj.appUser.PhoneNumber,
            };

            var existingUser = await _userManager.FindByEmailAsync(user.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "Email Already Exists !");
            }

            var result = await _userManager.CreateAsync(user, obj.Password);


            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, SD.User);

                var verificationCode = new Random().Next(100000, 999999).ToString();

                await _userManager.AddClaimAsync(user, new Claim("EmailVerificationCode", verificationCode));

                await _emailSender.SendEmailAsync(user.Email, "Email Confirmation Code",
                    $"Your email confirmation code is: {verificationCode}");

                return RedirectToAction("VerifyEmailCode", new { userId = user.Id });
            }

            obj.CityList = _db.cities.Select(i => new SelectListItem
            {
                Text = i.name,
                Value = i.Id.ToString()
            });

            obj.Statelist = _db.states.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            return View(obj);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmailCode(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }

            var model = new VerifyEmailCodeViewModel { UserId = userId };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyEmailCode(VerifyEmailCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return View("Error");
            }

            var claims = await _userManager.GetClaimsAsync(user);
            var storedCode = claims.FirstOrDefault(c => c.Type == "EmailVerificationCode")?.Value;

            if (storedCode == null || storedCode != model.Code)
            {
                ModelState.AddModelError("", "Invalid or expired code.");
                return View(model);
            }

            var result = await _userManager.ConfirmEmailAsync(user, await _userManager.GenerateEmailConfirmationTokenAsync(user));
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            return View("Error");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResendEmailCode(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return RedirectToAction("Register");

            var claims = await _userManager.GetClaimsAsync(user);
            var lastRequestClaim = claims.FirstOrDefault(c => c.Type == "LastResendTime");
            DateTime lastRequestTime = lastRequestClaim != null ? DateTime.Parse(lastRequestClaim.Value) : DateTime.MinValue;

            if ((DateTime.UtcNow - lastRequestTime).TotalSeconds < 60)
            {
                TempData["ErrorMessage"] = "Please wait before requesting a new code.";
                return RedirectToAction("VerifyResetCode", new { email = user.Email });
            }

            var newCode = new Random().Next(100000, 999999).ToString();
            await _userManager.SetAuthenticationTokenAsync(user, "Default", "ResetCode", newCode);

            if (lastRequestClaim != null) await _userManager.RemoveClaimAsync(user, lastRequestClaim);
            await _userManager.AddClaimAsync(user, new Claim("LastResendTime", DateTime.UtcNow.ToString()));

            await _emailSender.SendEmailAsync(user.Email, "Email Reset Code", $"Your new code: {newCode}");

            TempData["Message"] = "A new code has been sent to your email.";
            return RedirectToAction("VerifyEmailCode", new { email = user.Email });
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult EmailConfirmationSuccess()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "No account found with this email.");
                return View();
            }

            // Generate a 6-digit code
            var resetCode = new Random().Next(100000, 999999).ToString();

            // Store the code in a temporary place (e.g., user claims)
            await _userManager.RemoveClaimsAsync(user, await _userManager.GetClaimsAsync(user));
            await _userManager.AddClaimAsync(user, new Claim("ResetCode", resetCode));

            // Send email using SendGrid
            await _emailSender.SendEmailAsync(user.Email, "Password Reset Code",
                $"Your password reset code is: {resetCode}");

            return RedirectToAction("VerifyResetCode", new { email = user.Email });
        }


        [HttpGet]
        public ActionResult VerifyResetCode(string email)
        {
            var model = new VerifyCodeViewModel { Email = email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyResetCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid request.");
                return View();
            }

            var claims = await _userManager.GetClaimsAsync(user);
            var storedCode = claims.FirstOrDefault(c => c.Type == "ResetCode")?.Value;

            if (storedCode == null || storedCode != model.Code)
            {
                ModelState.AddModelError("", "Invalid or expired code.");
                return View();
            }

            return RedirectToAction("ResetPassword", new { email = model.Email });
        }

        [HttpGet]
        public ActionResult ResetPassword(string email)
        {
            var model = new ResetPasswordViewModel
            {
                Email = email
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            var result = await _userManager.RemovePasswordAsync(user);
            if (result.Succeeded)
            {
                result = await _userManager.AddPasswordAsync(user, model.NewPassword);
                if (result.Succeeded)
                {
                    return RedirectToAction("ResetPasswordConfirmation");
                }
            }
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResendCode(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return RedirectToAction("ForgotPassword");

            var claims = await _userManager.GetClaimsAsync(user);
            var lastRequestClaim = claims.FirstOrDefault(c => c.Type == "LastResendTime");
            DateTime lastRequestTime = lastRequestClaim != null ? DateTime.Parse(lastRequestClaim.Value) : DateTime.MinValue;

            if ((DateTime.UtcNow - lastRequestTime).TotalSeconds < 60)
            {
                TempData["ErrorMessage"] = "Please wait before requesting a new code.";
                return RedirectToAction("VerifyResetCode", new { email = user.Email });
            }

            var newCode = new Random().Next(100000, 999999).ToString();
            await _userManager.SetAuthenticationTokenAsync(user, "Default", "ResetCode", newCode);

            if (lastRequestClaim != null) await _userManager.RemoveClaimAsync(user, lastRequestClaim);
            await _userManager.AddClaimAsync(user, new Claim("LastResendTime", DateTime.UtcNow.ToString()));

            await _emailSender.SendEmailAsync(user.Email, "Password Reset Code", $"Your new code: {newCode}");

            TempData["Message"] = "A new code has been sent to your email.";
            return RedirectToAction("VerifyResetCode", new { email = user.Email });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnurl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Auth", new { returnurl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnurl = null, string remoteError = null)
        {
            returnurl = returnurl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View(nameof(SignIn));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(SignIn));
            }

            //Sign in the user with this external login provider, if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                //update any authentication tokens
                await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
                return LocalRedirect(returnurl);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToAction("VerifyAuthenticatorCode", new { returnurl = returnurl });
            }
            else
            {
                //If the user does not have account, then we will ask the user to create an account.
                ViewData["ReturnUrl"] = returnurl;
                ViewData["ProviderDisplayName"] = info.ProviderDisplayName;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var name = info.Principal.FindFirstValue(ClaimTypes.Name);
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email, Name = name });

            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnurl = null)
        {
            returnurl = returnurl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                //get the info about the user from external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("Error");
                }
                var user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
                        return LocalRedirect(returnurl);
                    }
                }
            }
            ViewData["ReturnUrl"] = returnurl;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> logOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
