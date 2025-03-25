using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrendsValley.Models.ViewModels;

namespace TrendsValley.Models.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
       
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public Product Product { get; set; }

        [NotMapped]
        public ProductDetailsViewModel ProductDetailsViewModel { get; set; }

        [ForeignKey("appUser")]

        public string UserId { get; set; }

        public AppUser appUser { get; set; }
    }
}
