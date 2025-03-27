using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace TrendsValley.Models.ViewModels
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        public string FName { get; set; }

        [Required]
        public string LName { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public int CityId { get; set; }

        [Required]
        public int StateId { get; set; }


        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PostalCode { get; set; }

        [Required]
        public string StreetAddress { get; set; }

        public IEnumerable<SelectListItem> Statelist { get; set; }

        public IEnumerable<SelectListItem> CityList { get; set; }

    }
}
