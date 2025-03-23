using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendsValley.Models.ViewModels
{
    public class ProfileViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string Fname { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string Lname { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Address")]
        public string StreetAddress { get; set; }


        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Display(Name = "Profile Image")]
        public string ProfileImageUrl { get; set; }

        [Display(Name = "state")]
        public string state { get; set; }

        [Display(Name = "City")]
        public string city { get; set; }

        public List<SelectListItem> GenderOptions { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "Male", Text = "Male" },
            new SelectListItem { Value = "Female", Text = "Female" }
        };
        public IEnumerable<SelectListItem> Statelist { get; set; }

        public IEnumerable<SelectListItem> CityList { get; set; }
    }
}
