using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendsValley.Models.Models
{
    public class UserDevice
    {
        public Guid? Id { get; set; } = Guid.NewGuid();

        [ForeignKey("User")]
        public string? UserId { get; set; }

        public string? DeviceName { get; set; } // مثال: "iPhone 13, Chrome"
        public string? DeviceType { get; set; } // "Mobile", "Desktop", etc.
        public string? OS { get; set; } // "iOS", "Android", "Windows"
        public string? Browser { get; set; } // "Chrome", "Safari"
        public string? IpAddress { get; set; }
        public string? Location { get; set; } // مدينة الدخول
        public DateTime? FirstLoginDate { get; set; } = DateTime.Now;
        public DateTime? LastLoginDate { get; set; } = DateTime.Now;
        public string? DeviceToken { get; set; } // Unique identifier للجهاز

        // Navigation property
        public virtual AppUser? User { get; set; }
    }
}
