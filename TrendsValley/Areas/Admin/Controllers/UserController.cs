using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TrendsValley.DataAccess.Data;
using TrendsValley.Models.Models;

namespace TrendsValley.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _db;

        public UserController(UserManager<AppUser> userManager, AppDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }
        public IActionResult Index()
        {
            var UserAccount = _db.appUsers.ToList();
            var userRole = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();
            foreach (var user in UserAccount)
            {
                var role = userRole.FirstOrDefault(u => u.UserId == user.Id);
                if (role == null)
                {
                    user.Role = "None";
                }
                else
                {
                    user.Role = roles.FirstOrDefault(u => u.Id == role.RoleId).Name;
                }
            }
            return View(UserAccount);
        }
    }
}
