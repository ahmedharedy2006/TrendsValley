using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrendsValley.DataAccess.Data;
using TrendsValley.DataAccess.Repository.Interfaces;
using TrendsValley.Models.Models;

namespace TrendsValley.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class StateController : Controller
    {
        private readonly IStateRepo _stateRepoo;
        public StateController(IStateRepo stateRepo)
        {
            _stateRepoo = stateRepo;
        }

        // GET All States Method and View
        public async Task<IActionResult> Index()
        {
            // Get All States from Database
            List<State> objList = await _stateRepoo.GetAllAsync();

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
            if (obj == null)
            {
                //Create a new State object
                await _stateRepoo.CreateAsync(obj);
            }
            else
            {
                //Update the existing State object
                await _stateRepoo.UpdateAsync(obj);
            }

            //redirect to the Index action
            return RedirectToAction(nameof(Index));
        }

        // GET Delete Method and View
        public async Task<IActionResult> Delete(int id)
        {
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
            }

            //redirect to the Index action
            return RedirectToAction(nameof(Index));
        }
    }
}
