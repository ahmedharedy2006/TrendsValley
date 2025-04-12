using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrendsValley.DataAccess.Data;
using TrendsValley.Models.Models;

namespace TrendsValley.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ActivityController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;

        public ActivityController(AppDbContext db , UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            var RecentSecurityActivities = await _db.AdminActivities
                .Include(a => a.User)
                .OrderByDescending(a => a.ActivityDate)
                .ToListAsync();

            return View(RecentSecurityActivities);
        }
            public async Task<IActionResult> Search(
        string searchTerm,
        string nameSearch,
        string fromDate,
        string toDate)
            {
            var query = _db.AdminActivities
                .Include(a => a.User)
                .AsQueryable();

            // Apply general search if provided
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(a =>
                    a.Description.Contains(searchTerm) ||
                    a.User.Fname.Contains(searchTerm) ||
                    a.User.Lname.Contains(searchTerm));
            }

            // Apply name search if provided
            if (!string.IsNullOrEmpty(nameSearch))
            {
                query = query.Where(a =>
                    a.User.Fname.Contains(nameSearch) ||
                    a.User.Lname.Contains(nameSearch));
            }

            // Apply date range filter if provided
            if (!string.IsNullOrEmpty(fromDate) && DateTime.TryParse(fromDate, out var fromDateValue))
            {
                query = query.Where(a => a.ActivityDate >= fromDateValue);
            }

            if (!string.IsNullOrEmpty(toDate) && DateTime.TryParse(toDate, out var toDateValue))
            {
                // Add one day to include the entire toDate day
                query = query.Where(a => a.ActivityDate < toDateValue.AddDays(1));
            }

            var model = await query.ToListAsync();
            return View("Index", model);
        }
    }
}
