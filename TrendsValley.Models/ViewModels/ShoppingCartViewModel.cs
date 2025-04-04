﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrendsValley.Models.Models;

namespace TrendsValley.Models.ViewModels
{
    public class ShoppingCartViewModel
    {
        public IEnumerable<ShoppingCart> ListCart { get; set; }

        public OrderHeader OrderHeader { get; set; }

        public string city { get; set; }

        public string state { get; set; }

        public IEnumerable<SelectListItem> Statelist { get; set; }

        public IEnumerable<SelectListItem> CityList { get; set; }
    }
}
