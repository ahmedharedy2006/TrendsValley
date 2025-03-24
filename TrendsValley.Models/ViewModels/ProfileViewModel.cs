using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrendsValley.Models.Models;

namespace TrendsValley.Models.ViewModels
{
    public class ProfileViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "Current City")]
        public string CurrentCity { get; set; }

        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Display(Name = "Gender")]
        public string Gender { get; set; }

        public AppUser User { get; set; }
        public List<SelectListItem> GenderOptions { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "Male", Text = "Male" },
            new SelectListItem { Value = "Female", Text = "Female" }
        };

        [Display(Name = "Profile Image")]
        public string ProfileImageUrl { get; set; }
    }
}
