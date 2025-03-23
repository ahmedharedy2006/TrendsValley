using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TrendsValley.Models.Models;

namespace TrendsValley.Models.Models
{
    public class Profile
    {
            [Key]
            public int Id { get; set; }

            [Required]
            public string UserId { get; set; } // Foreign key to ApplicationUser

            [ForeignKey("UserId")]
            public AppUser User { get; set; }

            [Required]
            public string Name { get; set; } = "Unknown"; // Default value

            public string? Address { get; set; } // Nullable

            public string? CurrentCity { get; set; } // Nullable

            public string? PostalCode { get; set; } // Nullable

            public string? Gender { get; set; } // Nullable

            public string? ProfileImageUrl { get; set; } // Nullable
        }
}