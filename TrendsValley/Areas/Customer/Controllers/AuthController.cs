using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Numerics;
using System.Security.Claims;
using System.Text.Json;
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
            UserManager<AppUser> userManager, AppDbContext db, IEmailSender emailSender)
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
        public async Task<IActionResult> SignIn(LoginViewModel obj)
        {
            if (!ModelState.IsValid) return View(obj);

            var user = await _userManager.FindByEmailAsync(obj.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, obj.Password))
            {
                ModelState.AddModelError("", "Invalid login attempt");
                return View(obj);
            }

            if (await _userManager.GetTwoFactorEnabledAsync(user))
            {
                // Generate code
                var code = new Random().Next(100000, 999999).ToString();
                var emailBody = GenerateEmailConfirmationEmail(user, code);

                // Store in session
                HttpContext.Session.SetString("2FA_Code", code);
                HttpContext.Session.SetString("2FA_User", user.Id);

                try
                {
                    await _emailSender.SendEmailAsync(
                        user.Email,
                        "Your Login Verification Code",
                        emailBody
                    );
                }
                catch
                {
                    ModelState.AddModelError("", "Failed to send verification email");
                    return View(obj);
                }
                return RedirectToAction("Enter2FACode");
            }
            var deviceInfo = new
            {
                DeviceName = GetFriendlyDeviceName(Request.Headers["User-Agent"]),
                DeviceType = GetDeviceType(Request.Headers["User-Agent"]),
                IP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                OS = GetOSFromUserAgent(Request.Headers["User-Agent"]),
                Browser = GetBrowserFromUserAgent(Request.Headers["User-Agent"]),
                Location = await GetLocationFromIP(HttpContext.Connection.RemoteIpAddress?.ToString())
            };

            var existingDevice = await _db.UserDevices
                .FirstOrDefaultAsync(d => d.UserId == user.Id &&
                                        d.DeviceToken == Request.Cookies["DeviceToken"]);

            if (existingDevice != null)
            {
                // لو الجهاز معروف قبل كده، نحدث تاريخ الدخول بس
                existingDevice.LastLoginDate = DateTime.Now;
            }
            else
            {
                // لو جهاز جديد، نضيفه في الداتابيز
                var newDevice = new UserDevice
                {
                    UserId = user.Id,
                    DeviceName = deviceInfo.DeviceName,
                    DeviceType = deviceInfo.DeviceType,
                    IpAddress = deviceInfo.IP,
                    OS = deviceInfo.OS,
                    Browser = deviceInfo.Browser,
                    Location = deviceInfo.Location,
                    DeviceToken = Guid.NewGuid().ToString(),
                    FirstLoginDate = DateTime.Now,
                    LastLoginDate = DateTime.Now
                };

                _db.UserDevices.Add(newDevice);

                // نحفظ التوكن في الكوكيز عشان نعرفه بعد كده
                Response.Cookies.Append("DeviceToken", newDevice.DeviceToken, new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(30),
                    HttpOnly = true,
                    Secure = true
                });
            }
            await _db.SaveChangesAsync();
            await _signInManager.SignInAsync(user, obj.RememberMe);
            return RedirectToAction("Index", "Home");
        }
        private string GetOSFromUserAgent(string userAgent)
        {
            if (userAgent.Contains("Windows")) return "Windows";
            if (userAgent.Contains("Mac")) return "MacOS";
            if (userAgent.Contains("Linux")) return "Linux";
            if (userAgent.Contains("Android")) return "Android";
            if (userAgent.Contains("iPhone")) return "iOS";
            return "Unknown";
        }

        private string GetBrowserFromUserAgent(string userAgent)
        {
            // Edge بيبقي عنده كل من "Edg" و"Chrome" في الـ User Agent
            if (userAgent.Contains("Edg"))
                return "Microsoft Edge";

            // الفحص علي Chrome لازم يكون بعد Edge
            if (userAgent.Contains("Chrome"))
                return "Google Chrome";

            if (userAgent.Contains("Firefox"))
                return "Mozilla Firefox";

            if (userAgent.Contains("Safari") && !userAgent.Contains("Chrome"))
                return "Apple Safari";

            if (userAgent.Contains("Opera") || userAgent.Contains("OPR"))
                return "Opera";

            return "Unknown Browser";
        }
        private string GetFriendlyDeviceName(string userAgent)
        {
            string deviceType = userAgent.Contains("Mobile") ? "Mobile" : "Desktop";

            if (userAgent.Contains("Windows NT"))
                deviceType = "Windows " + deviceType;
            else if (userAgent.Contains("Macintosh"))
                deviceType = "Mac " + deviceType;
            else if (userAgent.Contains("Linux"))
                deviceType = "Linux " + deviceType;

            string browser = GetBrowserFromUserAgent(userAgent);

            return $"{deviceType} ({browser})";
        }
        private string GetDeviceType(string userAgent)
        {
            // Mobile devices
            if (userAgent.Contains("Mobi") || userAgent.Contains("Android"))
                return "Mobile";

            // Tablets
            if (userAgent.Contains("Tablet") || userAgent.Contains("iPad"))
                return "Tablet";

            // Common desktop patterns
            if (userAgent.Contains("Windows NT") || userAgent.Contains("Macintosh") || userAgent.Contains("Linux"))
                return "Desktop";

            // Gaming consoles
            if (userAgent.Contains("Xbox") || userAgent.Contains("PlayStation"))
                return "Gaming Console";

            return "Unknown Device";
        }

        private async Task<string> GetLocationFromIP(string ipAddress)
        {
            // إذا كان IP محلي
            if (ipAddress == "::1" || ipAddress == "127.0.0.0")
            {
                // إرجاع موقع افتراضي
                return "Cairo, Egypt (Local Development)";
            }

            try
            {
                using var httpClient = new HttpClient();
                var response = await httpClient.GetFromJsonAsync<IpApiResponse>($"http://ip-api.com/json/{ipAddress}");

                return response switch
                {
                    { Status: "success" } => $"{response.City}, {response.Country}",
                    _ => "Unknown Location"
                };
            }
            catch
            {
                return "Location Unknown";
            }
        }

        private record IpApiResponse(
            string Status,
            string Country,
            string City
        );




        private string GenerateEmailConfirmationEmail(AppUser user, string confirmationCode)
        {
            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 20px;
        }}
        .email-container {{
            max-width: 600px;
            margin: 0 auto;
            background-color: #ffffff;
            padding: 30px;
            border-radius: 8px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
        }}
        .header {{
            text-align: center;
            margin-bottom: 25px;
            border-bottom: 1px solid #eaeaea;
            padding-bottom: 15px;
        }}
        .header h1 {{
            color: #6366f1;
            margin: 0;
            font-size: 24px;
        }}
        .content {{
            margin-bottom: 25px;
            line-height: 1.6;
        }}
        .content p {{
            font-size: 16px;
            color: #333333;
            margin-bottom: 15px;
        }}
        .verification-code {{
            font-size: 28px;
            font-weight: bold;
            color: #6366f1;
            letter-spacing: 3px;
            text-align: center;
            margin: 25px 0;
            padding: 15px;
            background-color: #f8f9fa;
            border-radius: 6px;
            border: 1px dashed #6366f1;
        }}
        .security-alert {{
            background-color: #f8f9fa;
            border-left: 4px solid #6366f1;
            padding: 15px;
            margin: 20px 0;
            border-radius: 4px;
        }}
        .footer {{
            text-align: center;
            font-size: 14px;
            color: #777;
            margin-top: 25px;
            border-top: 1px solid #eaeaea;
            padding-top: 15px;
        }}
        .button {{
            display: inline-block;
            padding: 12px 24px;
            background-color: #6366f1;
            color: white;
            text-decoration: none;
            border-radius: 4px;
            margin: 20px auto;
            text-align: center;
        }}
        .info-item {{
            margin-bottom: 8px;
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='header'>
            <h1>Two-Factor Authentication Code</h1>
        </div>
        
        <div class='content'>
            <p>Hello {user.Fname + " " + user.Lname},</p>
            
            <p>Your login attempt requires verification. Use this code to complete your sign-in:</p>
            
            <div class='verification-code'>
                {confirmationCode}
            </div>
            
            <p>This code will expire in 15 minutes. If you didn't request this, please ignore this email.</p>
            
            <div class='security-alert'>
                <p><strong>Security Tip:</strong> Never share this code with anyone. Trendsvalley will never ask for your verification code.</p>
            </div>
            
            <p>Alternatively, you can click the button below to verify your email:</p>
            
            <a href=""{GenerateVerificationLink(user, confirmationCode)}"" class='button'>Verify Email Address</a>
            
            <p>If the button doesn't work, copy and paste this link into your browser:</p>
            <p style=""word-break: break-all;"">{GenerateVerificationLink(user, confirmationCode)}</p>
        </div>
        
        <div class='footer'>
            <p>&copy; {DateTime.Now.Year} Cara-Store. All rights reserved.</p>
            <p>This email was sent to {user.Email} as part of our verification process.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GenerateVerificationLink(AppUser user, string code)
        {
            return Url.Action(
                "Enter2FACode",
                "Auth",
                new { userId = user.Id, code = code },
                protocol: HttpContext.Request.Scheme
            );
        }
        [HttpGet]
        public IActionResult Enter2FACode() => View();

        [HttpPost]
        public async Task<IActionResult> Verify2FA(string code)
        {
            var userId = HttpContext.Session.GetString("2FA_User");
            var correctCode = HttpContext.Session.GetString("2FA_Code");

            // If code matches, log the user in
            if (userId != null && code == correctCode)
            {
                var user = await _userManager.FindByIdAsync(userId);
                await _signInManager.SignInAsync(user, isPersistent: true);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Wrong code";
            return View("Enter2FACode");
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
                return RedirectToAction("EmailConfirmationSuccess", "Home");
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
