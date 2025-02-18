using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrendsValley.Models.Models;

namespace TrendsValley.Models.ViewModels
{
    public class ProductViewModel
    {
        
            public Product product { get; set; }
            public IEnumerable<SelectListItem> BrandList { get; set; }
            public IEnumerable<SelectListItem> CategoryList { get; set; }

    }
}
