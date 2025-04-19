using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TrendsValley.DataAccess.Data;
using TrendsValley.Models.Models;
using TrendsValley.Utilities;

namespace TrendsValley.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDbContext _db;

        public UserController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager , AppDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        // GET: All Users method and view
        public IActionResult Index(int pg = 1)
        {
            // Get all users from the database
            var UserAccount = _db.appUsers.ToList();

            // Get all user roles and roles
            var userRole = _db.UserRoles.ToList();

            //Get all roles from the database
            var roles = _db.Roles.ToList();

            // Loop through each user and assign the role
            foreach (var user in UserAccount)
            {
                // Get the role for the user
                var role = userRole.FirstOrDefault(u => u.UserId == user.Id);

                // If the role is null, assign "None" to the user role
                if (role == null)
                {
                    user.Role = SD.User;
                }
                else
                {
                    user.Role = roles.FirstOrDefault(u => u.Id == role.RoleId).Name;
                }
            }

            // Pagination logic
            const int pageSize = 8;
            if (pg < 1) pg = 1;

            int recsCount = UserAccount.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;

            UserAccount = UserAccount.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;
            ViewBag.count = recsCount;  
            // Return the list of users to the view
            return View(UserAccount);
        }

        // GET: Create and Edit method and view
        public IActionResult Edit(string userId)
        {
            //get the user from the database
            var objFromDb = _db.appUsers.FirstOrDefault(u => u.Id == userId);

            // If the user is null, return NotFound
            if (objFromDb == null)
            {
                return NotFound();
            }

            // Get all user roles and roles
            var userRole = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();
            var role = userRole.FirstOrDefault(u => u.UserId == objFromDb.Id);

            // If the role is not null, assign the role ID to the user
            if (role != null)
            {
                objFromDb.RoleId = roles.FirstOrDefault(u => u.Id == role.RoleId).Id;

            }
            objFromDb.RoleList = _db.Roles.Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = u.Name,
                Value = u.Id
            });

            // Return the user object to the view
            return View(objFromDb);
        }

        //Get : Make Admin method
        public async Task<IActionResult> makeAdmin(string userId)
        {
            // Get the user from the database
            var user = await _userManager.FindByIdAsync(userId);

            // If the user is null, return NotFound
            if (user == null)
            {
                return NotFound();
            }

            // Check if the user is already in the Admin role
            if (await _userManager.IsInRoleAsync(user, SD.Admin))
            {
                // If the user is already in the Admin role, remove them from the role
                await _userManager.RemoveFromRoleAsync(user, SD.Admin);

                //redirect to the Index action
                return RedirectToAction(nameof(Index));
            }

            // If the user is not in the Admin role, add them to the role
            await _userManager.AddToRoleAsync(user, SD.Admin);

            //redirect to the Index action
            return RedirectToAction(nameof(Index));
        }

        //GET: lock and unlock method
        public IActionResult LockUnlock(string userId)
        {
            // Get the user from the database
            var objFromDb = _db.appUsers.FirstOrDefault(u => u.Id == userId);

            // If the user is null, return NotFound
            if (objFromDb == null)
            {
                return NotFound();
            }

            // Check if the user is already locked out
            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                objFromDb.LockoutEnd = DateTime.Now;
            }
            // If the user is not locked out, lock them out
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddDays(10);
            }

            // Update the user in the database
            _db.SaveChanges();

            // Redirect to the Index action
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var CurrentuserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (user == null)
            {
                return NotFound();
            }

            if (User.IsInRole(SD.Admin))
            {
                return RedirectToAction("Index");
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
                

                if (userId == CurrentuserId)
                {
                    await _signInManager.SignOutAsync();
                }

                
                return RedirectToAction("Index");
            }
            return View("Error");
        }
    }
}
