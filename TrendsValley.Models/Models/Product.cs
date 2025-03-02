using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendsValley.Models.Models
{
    public class Product
    {
        [Key]
        public int Product_Id { get; set; }
        [Required]
        public string Product_Name { get; set; }
        public string Product_Details { get; set; }
        public string imgUrl { get; set; }
        public decimal Product_Price { get; set; }

        //brand
        [ForeignKey("Product_Brand")]
        public int Brand_Id { get; set; }
        public Brand Product_Brand { get; set; }
        //category
        [ForeignKey("Product_Category")]
        public int Category_id { get; set; }
        public Category Product_Category { get; set; }

    }
}
