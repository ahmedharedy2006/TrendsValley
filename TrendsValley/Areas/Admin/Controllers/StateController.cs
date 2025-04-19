using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TrendsValley.DataAccess.Data;
using TrendsValley.DataAccess.Repository;
using TrendsValley.DataAccess.Repository.Interfaces;
using TrendsValley.Models.Models;

namespace TrendsValley.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StateController : Controller
    {
        private readonly IStateRepo _stateRepoo;
        private readonly UserManager<AppUser> _userManager;
        public StateController(IStateRepo stateRepo,UserManager<AppUser> userManager)
        {
            _stateRepoo = stateRepo;
            _userManager = userManager;
        }

        // GET All States Method and View
        [Area("Admin")]
        [HttpGet("[area]/[controller]/[action]")]

        public async Task<IActionResult> Index(int pg = 1,string searchTerm = "")
        {
            // Get All States from Database
            List<State> objList = await _stateRepoo.GetAllAsync();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                objList = objList.Where(b =>
                    b.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            // Pagination logic
            const int pageSize = 8;
            if (pg < 1) pg = 1;

            int recsCount = objList.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;

            objList = objList.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;
            ViewBag.Count = recsCount;
            // Return the list to the view
            return View(objList);
        }

        // GET Create and Edit Method and View
        public async Task<IActionResult> Upsert(int? id)
        {
            // Create a new State object
            State obj = new();

            // If id is null or 0, return the view with the new object
            if (id == null || id == 0)
            {
                return View(obj);
            }
            // If id is not null or 0, get the State object from the database
            obj = await _stateRepoo.GetAsync(u => u.Id == id);

            // If the object is null, return NotFound
            if (obj == null)
            {
                return NotFound();
            }
            // If the object is not null, return the view with the object
            return View(obj);
        }

        // POST Create and Edit Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(State obj)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            if (obj.Id == null)
            {
                //Create a new State object
                await _stateRepoo.CreateAsync(obj);
                await _stateRepoo.AdminActivityAsync(
                userId: user.Id,
                activityType: "AddState",
                description: $"Add State(Id: {obj.Id})",
                ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString()
                );
            }
            else
            {
                //Update the existing State object
                await _stateRepoo.UpdateAsync(obj);
                await _stateRepoo.AdminActivityAsync(
                userId: user.Id,
                activityType: "UpdateState",
                description: $"Update State(Id: {obj.Id})",
                ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString()
                );
            }

            //redirect to the Index action
            return RedirectToAction(nameof(Index));
        }

        // GET Delete Method and View
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            // Get the State object from the database
            State obj = new();

            // Get the State object from the database
            obj = await _stateRepoo.GetAsync(s => s.Id == id);

            // If the object is null, return NotFound
            if (obj == null)
            {
                return NotFound();
            }
            else
            {
                //remove the State object from the database
                await _stateRepoo.RemoveAsync(obj);
                await _stateRepoo.AdminActivityAsync(
                userId: user.Id,
                activityType: "RemoveState",
                description: $"Remove State(Id: {obj.Id})",
                ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString()
                );
            }

            //redirect to the Index action
            return RedirectToAction(nameof(Index));
        }
    }
}
