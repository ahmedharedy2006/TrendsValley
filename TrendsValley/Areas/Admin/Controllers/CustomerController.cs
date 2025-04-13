using Microsoft.AspNetCore.Mvc;
using TrendsValley.DataAccess.Data;
using TrendsValley.Models.Models;
using TrendsValley.Utilities;

namespace TrendsValley.Areas.Admin.Controllers
{
    public class CustomerController : Controller
    {
        private readonly AppDbContext _db;
        public CustomerController(AppDbContext db)
        {
            _db = db;
        }
        // GET: All Users method and view
        public IActionResult Index(int pg = 1)
        {
            // Get all users from the database
                var customers = _db.customers.ToList();
        
            // Pagination logic
            const int pageSize = 8;
            if (pg < 1) pg = 1;
            int recsCount = customers.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            customers = customers.Skip(recSkip).Take(pager.PageSize).ToList();
            ViewBag.Pager = pager;
            ViewBag.count = recsCount;
            return View(customers);
        }
    }
   
}
