﻿using TrendsValley.Models.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TrendsValley.Models.Models
{
    public class AppUser : IdentityUser
    {
        [Required]
        public string Fname { get; set; }

        [Required]
        public string Lname { get; set; }

        [Required]
        public string StreetAddress { get; set; }
        public string? StreetAddress2 { get; set; }
        public string? SelectedAddress { get; set; }

        [NotMapped]
        public string? NewStreetAddress { get; set; }

        [NotMapped]
        public string? NewStreetAddress2 { get; set; }
        public string? PreferredLanguage { get; set; } = "en";
        public string? Currency { get; set; } = "USD";
        public string? PaymentMehtod { get; set; } = "Cash";
        public string? PreferredCarriers { get; set; } = "Bosta";


        [Required]
        [ForeignKey("city")]
        public int CityId { get; set; }

        public City city { get; set; }

        [Required]
        [ForeignKey("state")]
        public int StateId { get; set; }

        public State state { get; set; }

        [Required]
        public string PostalCode { get; set; }

        [NotMapped]
        public string RoleId { get; set; }
        [NotMapped]
        public string Role { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem> RoleList { get; set; }
    }
}
