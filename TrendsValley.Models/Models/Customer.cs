using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendsValley.Models.Models
{
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        [Required]
        public string Fname { get; set; }

        [Required]
        public string Lname { get; set; }

        [Required]
        public string StreetAddress { get; set; }
        public string? StreetAddress2 { get; set; }
        public string? SelectedAddress { get; set; }

        public string? PreferredLanguage { get; set; } = "en";
        public string? Currency { get; set; } = "USD";
        public string? PaymentMehtod { get; set; } = "Cash";
        public string? PreferredCarriers { get; set; } = "Bosta";

        [Required]
        public string phoneNumber { get; set; }

        [Required]
        [ForeignKey("city")]
        public int CityId { get; set; }

        public City city { get; set; }

        [Required]
        [ForeignKey("state")]
        public int StateId { get; set; }

        public State state { get; set; }

    }
}
