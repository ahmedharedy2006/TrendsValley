using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendsValley.Models.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        [Range(1, 1000, ErrorMessage = "Please Enter A Value Between 1 And 1000")]

        public int Quantity { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public Product Product { get; set; }

        [ForeignKey("appUser")]

        public string UserId { get; set; }

        public AppUser appUser { get; set; }
    }
}
