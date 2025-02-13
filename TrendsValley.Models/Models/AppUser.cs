using BooksMine.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
