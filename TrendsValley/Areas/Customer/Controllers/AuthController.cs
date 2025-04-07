using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Net;
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
    public class AuthController : BaseController
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
                var emailBody = GenerateEmail2FA(user, code);

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
            await TrackUserDevice(user);

            await _signInManager.SignInAsync(user, obj.RememberMe);
            if (HttpContext.Session != null)
            {
                HttpContext.Session.SetString("lang", user.PreferredLanguage ?? "en");
            }

            return RedirectToAction("Index", "Home");
        }


        private string GenerateEmail2FA(AppUser user, string confirmationCode)
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
            
            <a href=""{Generate2FALink(user, confirmationCode)}"" class='button'>Verify Email Address</a>
            
            <p>If the button doesn't work, copy and paste this link into your browser:</p>
            <p style=""word-break: break-all;"">{Generate2FALink(user, confirmationCode)}</p>
        </div>
        
        <div class='footer'>
            <p>&copy; {DateTime.Now.Year} Cara-Store. All rights reserved.</p>
            <p>This email was sent to {user.Email} as part of our verification process.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string Generate2FALink(AppUser user, string code)
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

            if (userId != null && code == correctCode)
            {
                var user = await _userManager.FindByIdAsync(userId);

                await TrackUserDevice(user);

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
                var emailBody = GenerateEmailConfirmationEmail(user, verificationCode);
                await _userManager.AddClaimAsync(user, new Claim("EmailVerificationCode", verificationCode));

                await _emailSender.SendEmailAsync(user.Email, "Email Confirmation Code",
                    emailBody);

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
            <h1>Email Verification</h1>
        </div>
        
        <div class='content'>
            <p>Hello {user.Fname + " " + user.Lname},</p>
            
            <p>Thank you for registering with Trendsvalley! Please use the following verification code to confirm your email address:</p>
            
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
                "VerifyEmailCode",
                "Auth",
                new { userId = user.Id, code = code },
                protocol: HttpContext.Request.Scheme
            );
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

        public async Task<IActionResult> DeleteAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            var userDervices = await _db.UserDevices.Where(u => u.UserId == userId).ToListAsync();
            var carts = await _db.Carts.Where(u => u.UserId == userId).ToListAsync();
            var orders = await _db.OrderHeaders.Where(u => u.AppUserId == userId).ToListAsync();

            _db.UserDevices.RemoveRange(userDervices);
            _db.Carts.RemoveRange(carts);
            _db.OrderHeaders.RemoveRange(orders);
            await _db.SaveChangesAsync();

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Index", "Home");
            }
            return View("Error");
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

            var ipAddress = GetClientIpAddress();
            var deviceName = System.Net.Dns.GetHostName();
            var changeTime = DateTime.UtcNow;

            var resetCode = new Random().Next(100000, 999999).ToString();
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink = passwordResetLink(user, resetToken); 

            await _userManager.RemoveClaimsAsync(user, await _userManager.GetClaimsAsync(user));
            await _userManager.AddClaimAsync(user, new Claim("ResetCode", resetCode));

            var emailBody = GenerateForgotPasswordEmail(user, ipAddress, deviceName, changeTime, resetCode, resetLink);

            await _emailSender.SendEmailAsync(user.Email, "Password Reset Code", emailBody);

            return RedirectToAction("VerifyResetCode", new { email = user.Email });
        }

        private string GenerateForgotPasswordEmail(
            AppUser user,
            string ipAddress,
            string deviceName,
            DateTime requestTime,
            string verificationCode,
            string passwordResetLink)
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
            font-size: 24px;
            font-weight: bold;
            color: #6366f1;
            letter-spacing: 3px;
            text-align: center;
            margin: 20px 0;
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
            padding: 10px 20px;
            background-color: #6366f1;
            color: white;
            text-decoration: none;
            border-radius: 4px;
            margin: 15px 0;
        }}
        .info-item {{
            margin-bottom: 8px;
        }}
        .info-label {{
            font-weight: bold;
            color: #555;
        }}
        .security-message {{
            color: #6366f1;
            font-weight: bold;
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='header'>
            <h1>Password Reset Verification</h1>
        </div>
        
        <div class='content'>
            <p>Hello {user.Fname + " " + user.Lname},</p>
            
            <p class='security-message'>A password reset was requested for your account!</p>
            
            <div class='security-alert'>
                <p><strong>SECURITY NOTICE:</strong> Use this verification code to confirm your identity:</p>
                <div class='verification-code'>
                    {verificationCode}
                </div>
                <p>This code expires in 15 minutes.</p>
            </div>
            
            <div class='security-alert'>
                <p><strong>IMPORTANT:</strong> Do not share this code with anyone. TrendsValley will never ask for your verification code.</p>
            </div>
            
            <p>Request details:</p>
            <div class='info-item'>
                <span class='info-label'>Device:</span> {deviceName}
            </div>
            <div class='info-item'>
                <span class='info-label'>IP Address:</span> {ipAddress}
            </div>
            <div class='info-item'>
                <span class='info-label'>Time:</span> {requestTime:f}
            </div>
            
            <p>To complete your password reset, click the button below:</p>
            
            <a href=""{passwordResetLink}"" class='button'>Reset Password</a>
            
            <p style=""word-break: break-all;"">Or copy this link manually: {passwordResetLink}</p>
            
            <p>If you didn't request this password reset, please secure your account immediately.</p>
        </div>
        
        <div class='footer'>
            <p>&copy; {requestTime.Year} TrendsValley. All rights reserved.</p>
            <p>This email was sent to {user.Email} as part of our security notifications.</p>
        </div>
    </div>
</body>
</html>";
        }
       
        private string passwordResetLink(AppUser user, string code)
        {
            return Url.Action(
                "ResetPassword",
                "Account",
                new { userId = user.Id, code = code },
                protocol: HttpContext.Request.Scheme
            );
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
                var activity = new SecurityActivity
                {
                    UserId = user.Id,
                    ActivityType = "PasswordChange",
                    Description = "Password changed",
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
                };
                _db.SecurityActivities.Add(activity);
                await _db.SaveChangesAsync();
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

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                await _signInManager.UpdateExternalAuthenticationTokensAsync(info);

                var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                await TrackUserDevice(user);

                return LocalRedirect(returnurl);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToAction("VerifyAuthenticatorCode", new { returnurl = returnurl });
            }
            else
            {
                ViewData["ReturnUrl"] = returnurl;
                ViewData["ProviderDisplayName"] = info.ProviderDisplayName;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel
                {
                    Email = email,
                    CityList = _db.cities.Select(i => new SelectListItem
                    {
                        Text = i.name,
                        Value = i.Id.ToString()
                    }),
                    Statelist = _db.states.Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    })
                });

            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnurl = null)
        {
            returnurl = returnurl ?? Url.Content("~/");


            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return View("Error");
            }
            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                CityId = model.CityId,
                StateId = model.StateId,
                PhoneNumber = model.Phone,
                StreetAddress = model.StreetAddress,
                Fname = model.FName,
                Lname = model.LName,
                PostalCode = model.PostalCode
            };
            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                result = await _userManager.AddLoginAsync(user, info);
                if (result.Succeeded)
                {
                    user.EmailConfirmed = true;
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
                    await TrackUserDevice(user);
                    return LocalRedirect(returnurl);
                }
            }

            ViewData["ReturnUrl"] = returnurl;
            return View(model);
        }

        private async Task TrackUserDevice(AppUser user)
        {
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
                existingDevice.LastLoginDate = DateTime.Now;
            }
            else
            {
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

                Response.Cookies.Append("DeviceToken", newDevice.DeviceToken, new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(30),
                    HttpOnly = true,
                    Secure = true
                });
            }

            var newDeviceActivity = new SecurityActivity
            {
                UserId = user.Id,
                ActivityType = "NewDeviceLogin",
                Description = "Logged in from new device",
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            };

            _db.SecurityActivities.Add(newDeviceActivity);
            await _db.SaveChangesAsync();
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
