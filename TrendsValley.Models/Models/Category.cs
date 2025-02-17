using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendsValley.Models.Models
{
    public class Category
    {
        [Key]
        public int Category_id { get; set; }
        [Required]
        public string Category_Name { get; set; }

    }
}
