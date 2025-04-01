using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Globalization;

namespace TrendsValley.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class BaseController : Controller
    {

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (HttpContext.Session != null)
            {
                string lang = HttpContext.Session.GetString("lang") ?? "en";

                CultureInfo culture = new CultureInfo(lang);
                Thread.CurrentThread.CurrentUICulture = culture;
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(lang);
            }

            base.OnActionExecuting(context);
        }
    }
}
