using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrendsValley.Models.Models;

namespace TrendsValley.Models.ViewModels
{
    public class ClaimsViewModel
    {
        public ClaimsViewModel()
        {
            ProductClaimList = [];
            CategoryClaimList = [];
            BrandClaimList = [];
            CityClaimList = [];
            StateClaimList = [];
            CustomerClaimList = [];
            AdminUserClaimList = [];

        }
        public IdentityRole Role { get; set; }
        public List<ClaimSelection> ProductClaimList { get; set; }
        public List<ClaimSelection> CategoryClaimList { get; set; }

        public List<ClaimSelection> BrandClaimList { get; set; }

        public List<ClaimSelection> CityClaimList { get; set; }

        public List<ClaimSelection> StateClaimList { get; set; }

        public List<ClaimSelection> CustomerClaimList { get; set; }

        public List<ClaimSelection> AdminUserClaimList { get; set; }
    }

    public class ClaimSelection
    {
        public string ClaimType { get; set; }
        public bool IsSelected { get; set; }
    }
}

