using System.Text.Json;

namespace TrendsValley.Services
{
    public class LocalizationService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private Dictionary<string, string> _translations = new();

        public LocalizationService(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        public void SetLanguage(string lang)
        {
            // Store the language in the session
            _httpContextAccessor.HttpContext.Session.SetString("lang", lang);
            Load(lang);
        }

        public void Load(string lang)
        {
            var path = Path.Combine(_env.ContentRootPath, "Resources", $"{lang}.json");

            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                _translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
            }
            else
            {
                _translations = new(); // fallback to an empty dictionary
            }
        }

        public string Translate(string key)
        {
            return _translations.TryGetValue(key, out var value) ? value : key;
        }

        public string GetCurrentLanguage()
        {
            // Retrieve the current language from the session or default to "en"
            return _httpContextAccessor.HttpContext.Session.GetString("lang") ?? "en";
        }
    }
}
