using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrendsValley.Models.Models;

namespace TrendsValley.Models.ViewModels
{
    public class ProductDetailsViewModel
    {
        public int Id { get; set; }
        public Product product { get; set; }
        public string CategoryName { get; set; }

        public string Brandname { get; set; }
    }
}
