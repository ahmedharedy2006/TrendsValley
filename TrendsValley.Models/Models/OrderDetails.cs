using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendsValley.Models.Models
{
    public class OrderDetails
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("orderHeader")]
        public int orderHeaderId { get; set; }

        public OrderHeader orderHeader { get; set; }

        [Required]
        [ForeignKey("Book")]
        public int productId { get; set; }

        public Product product { get; set; }

        public int count { get; set; }

        public double price { get; set; }
    }
}
