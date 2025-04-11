using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TrendsValley.DataAccess.Data
{
    public static class ClaimStore
    {
        public static List<Claim> ProductClaimList =
            [
                new Claim("Add Product" , "Add Product"),
                new Claim("View Product", "View Product"),
                new Claim("Edit Product", "Edit Product"),
                new Claim("Delete Product", "Delete Product"),
            ];


        public static List<Claim> CategoryClaimList =
            [

            new Claim("Add Category" , "Add Category"),
            new Claim("View Category", "View Category"),
            new Claim("Edit Category", "Edit Category"),
            new Claim("Delete Category", "Delete Category"),
        ];

        public static List<Claim> BrandClaimList =
            [
                new Claim("Add Brand" , "Add Brand"),
            new Claim("View Brand", "View Brand"),
            new Claim("Edit Brand", "Edit Brand"),
            new Claim("Delete Brand", "Delete Brand"),
        ];

        public static List<Claim> CityClaimsList =
            [
            new Claim("Add City" , "Add City"),
            new Claim("View City", "View City"),
            new Claim("Edit City", "Edit City"),
            new Claim("Delete City", "Delete City"),
        ];

        public static List<Claim> AdminUserClaimsList =
            [
            new Claim("Add Admin" , "Add Admin"),
            new Claim("View Admin", "View Admin"),
            new Claim("Edit Admin", "Edit Admin"),
            new Claim("Delete Admin", "Delete Admin"),
        ];

        public static List<Claim> CustomerClaimsList =
            [
            new Claim("Add User" , "Add User"),
            new Claim("View User", "View User"),
            new Claim("Edit User", "Edit User"),
            new Claim("Delete User", "Delete User"),
        ];

        public static List<Claim> StateClaimList = [
            new Claim("Add State" , "Add State"),
            new Claim("View State", "View State"),
            new Claim("Edit State", "Edit State"),
            new Claim("Delete State", "Delete State"),
        ];
    }
}
