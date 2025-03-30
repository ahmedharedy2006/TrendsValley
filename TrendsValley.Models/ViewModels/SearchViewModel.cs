using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrendsValley.Models.Models;

namespace TrendsValley.Models.ViewModels
{
    public class SearchViewModel
    {
        public string SearchTerm { get; set; }
        public List<Product> Results { get; set; }
    }
}
