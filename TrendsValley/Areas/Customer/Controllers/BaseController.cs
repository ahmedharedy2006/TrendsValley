using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Globalization;
using System.Net;
using TrendsValley.DataAccess.Data;
using TrendsValley.Models.Models;

namespace TrendsValley.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class BaseController : Controller
    {
        public string GetClientIpAddress()
        {
            var ip = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();

            if (string.IsNullOrEmpty(ip))
            {
                ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            }
            else
            {
                ip = ip.Split(',')[0].Trim();
            }

            if (IPAddress.TryParse(ip, out var address))
            {
                if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    ip = address.MapToIPv4().ToString();
                }
                else if (ip == "::1")
                {
                    ip = "127.0.0.1";
                }
            }

            return ip ?? "Unknown";
        }
        public string GetOSFromUserAgent(string userAgent)
        {
            if (userAgent.Contains("Windows")) return "Windows";
            if (userAgent.Contains("Mac")) return "MacOS";
            if (userAgent.Contains("Linux")) return "Linux";
            if (userAgent.Contains("Android")) return "Android";
            if (userAgent.Contains("iPhone")) return "iOS";
            return "Unknown";
        }

        public string GetBrowserFromUserAgent(string userAgent)
        {
            if (userAgent.Contains("Edg"))
                return "Microsoft Edge";

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
        public string GetFriendlyDeviceName(string userAgent)
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
        public string GetDeviceType(string userAgent)
        {
            if (userAgent.Contains("Mobi") || userAgent.Contains("Android"))
                return "Mobile";

            if (userAgent.Contains("Tablet") || userAgent.Contains("iPad"))
                return "Tablet";

            if (userAgent.Contains("Windows NT") || userAgent.Contains("Macintosh") || userAgent.Contains("Linux"))
                return "Desktop";

            if (userAgent.Contains("Xbox") || userAgent.Contains("PlayStation"))
                return "Gaming Console";

            return "Unknown Device";
        }

        public async Task<string> GetLocationFromIP(string ipAddress)
        {
            if (ipAddress == "::1" || ipAddress == "127.0.0.0")
            {
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

        public record IpApiResponse(
            string Status,
            string Country,
            string City
        );
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
