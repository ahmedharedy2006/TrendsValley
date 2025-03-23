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
        public string UserId { get; set; } 

        [ForeignKey("UserId")]
        public AppUser User { get; set; }

        [Required]
        public string Name { get; set; } = "Unknown"; 

        public string? Address { get; set; } 

        public string? CurrentCity { get; set; }

        public string? PostalCode { get; set; } 

        public string? Gender { get; set; } 

        public string? ProfileImageUrl { get; set; } 
    }
}