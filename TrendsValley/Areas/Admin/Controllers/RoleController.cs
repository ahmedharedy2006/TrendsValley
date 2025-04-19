using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrendsValley.DataAccess.Data;
using TrendsValley.Models.Models;
using TrendsValley.Models.ViewModels;
using TrendsValley.Utilities;

namespace TrendsValley.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(AppDbContext db, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index(int pg = 1)
        {
            var roles = _db.Roles.ToList();

            // Pagination logic
            const int pageSize = 8;
            if (pg < 1) pg = 1;

            int recsCount = roles.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;

            roles = roles.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;
            ViewBag.count = recsCount;

            return View(roles);
        }

        [HttpGet]
        public IActionResult Upsert(string roleId)
        {
            if (String.IsNullOrEmpty(roleId))
            {
                //create
                return View();
            }
            else
            {
                //update
                var objFromDb = _db.Roles.FirstOrDefault(u => u.Id == roleId);
                return View(objFromDb);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(IdentityRole roleObj)
        {
            if (await _roleManager.RoleExistsAsync(roleObj.Name))
            {
                //error
            }
            if (String.IsNullOrEmpty(roleObj.NormalizedName))
            {
                //create
                await _roleManager.CreateAsync(new IdentityRole() { Name = roleObj.Name });
                TempData[SD.Success] = "Role created successfully";
            }
            else
            {
                //update
                var objFromDb = _db.Roles.FirstOrDefault(u => u.Id == roleObj.Id);
                objFromDb.Name = roleObj.Name;
                objFromDb.NormalizedName = roleObj.Name.ToUpper();
                var result = await _roleManager.UpdateAsync(objFromDb);
                TempData[SD.Success] = "Role updated successfully";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ManageRoleClaim(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                return NotFound();
            }

            var existingUserClaims = await _roleManager.GetClaimsAsync(role);

            var model = new ClaimsViewModel()
            {
                Role = role
            };

            foreach (Claim claim in ClaimStore.ProductClaimList)
            {
                model.ProductClaimList.Add(new ClaimSelection
                {
                    ClaimType = claim.Type,
                    IsSelected = existingUserClaims.Any(c => c.Type == claim.Type)
                });
            }

            foreach (Claim claim in ClaimStore.CategoryClaimList)
            {
                model.CategoryClaimList.Add(new ClaimSelection
                {
                    ClaimType = claim.Type,
                    IsSelected = existingUserClaims.Any(c => c.Type == claim.Type)
                });
            }

            foreach (Claim claim in ClaimStore.BrandClaimList)
            {
                model.BrandClaimList.Add(new ClaimSelection
                {
                    ClaimType = claim.Type,
                    IsSelected = existingUserClaims.Any(c => c.Type == claim.Type)
                });
            }

            foreach (Claim claim in ClaimStore.CityClaimsList)
            {
                model.CityClaimList.Add(new ClaimSelection
                {
                    ClaimType = claim.Type,
                    IsSelected = existingUserClaims.Any(c => c.Type == claim.Type)
                });
            }

            foreach (Claim claim in ClaimStore.StateClaimList)
            {
                model.StateClaimList.Add(new ClaimSelection
                {
                    ClaimType = claim.Type,
                    IsSelected = existingUserClaims.Any(c => c.Type == claim.Type)
                });
            }

            foreach (Claim claim in ClaimStore.CustomerClaimsList)
            {
                model.CustomerClaimList.Add(new ClaimSelection
                {
                    ClaimType = claim.Type,
                    IsSelected = existingUserClaims.Any(c => c.Type == claim.Type)
                });
            }


            foreach (Claim claim in ClaimStore.AdminUserClaimsList)
            {
                model.AdminUserClaimList.Add(new ClaimSelection
                {
                    ClaimType = claim.Type,
                    IsSelected = existingUserClaims.Any(c => c.Type == claim.Type)
                });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRoleClaim(ClaimsViewModel claimsViewModel)
        {
            var role = await _roleManager.FindByIdAsync(claimsViewModel.Role.Id);

            if (role == null)
            {
                return NotFound();
            }

            // Get existing claims for comparison or removal if needed
            var oldClaims = await _roleManager.GetClaimsAsync(role);

            foreach (var claim in oldClaims)
            {
                await _roleManager.RemoveClaimAsync(role, claim); 
            }

            // Add the selected claims from each list
            var allClaims = new List<Claim>();

            allClaims.AddRange(claimsViewModel.ProductClaimList
                .Where(x => x.IsSelected)
                .Select(y => new Claim(y.ClaimType, y.ClaimType))); // ClaimType as value

            allClaims.AddRange(claimsViewModel.CategoryClaimList
                .Where(x => x.IsSelected)
                .Select(y => new Claim(y.ClaimType, y.ClaimType)));

            allClaims.AddRange(claimsViewModel.BrandClaimList
                .Where(x => x.IsSelected)
                .Select(y => new Claim(y.ClaimType, y.ClaimType)));

            allClaims.AddRange(claimsViewModel.CityClaimList
                .Where(x => x.IsSelected)
                .Select(y => new Claim(y.ClaimType, y.ClaimType)));

            allClaims.AddRange(claimsViewModel.StateClaimList
                .Where(x => x.IsSelected)
                .Select(y => new Claim(y.ClaimType, y.ClaimType)));

            allClaims.AddRange(claimsViewModel.CustomerClaimList
                .Where(x => x.IsSelected)
                .Select(y => new Claim(y.ClaimType, y.ClaimType)));

            allClaims.AddRange(claimsViewModel.AdminUserClaimList
                .Where(x => x.IsSelected)
                .Select(y => new Claim(y.ClaimType, y.ClaimType)));

            // Now add all selected claims
            IdentityResult result = IdentityResult.Success;
            foreach (var claim in allClaims)
            {
                result = await _roleManager.AddClaimAsync(role, claim);
                if (!result.Succeeded)
                {
                    TempData[SD.Error] = "Error while adding claims";
                    return View(claimsViewModel);
                }
            }

            TempData[SD.Success] = "Claims assigned successfully";
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(string roleId)
        {

            var objFromDb = _db.Roles.FirstOrDefault(u => u.Id == roleId);
            if (objFromDb != null)
            {

                var userRolesForThisRole = _db.UserRoles.Where(u => u.RoleId == roleId).Count();
                if (userRolesForThisRole > 0)
                {
                    TempData[SD.Error] = "Cannot delete this role, since there are users assigned to this role.";
                    return RedirectToAction(nameof(Index));
                }

                var result = await _roleManager.DeleteAsync(objFromDb);
                TempData[SD.Success] = "Role deleted successfully";
            }
            else
            {
                TempData[SD.Error] = "Role not found.";
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
