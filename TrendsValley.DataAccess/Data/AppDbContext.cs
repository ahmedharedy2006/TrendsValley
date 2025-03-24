using TrendsValley.Models.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrendsValley.Models.Models;

namespace TrendsValley.DataAccess.Data
{
    public class AppDbContext : IdentityDbContext
    {
        
        public AppDbContext(DbContextOptions options)
           : base(options)
        {
        }

        public DbSet<AppUser> appUsers { get; set; }

        public DbSet<State> states { get; set; }

        public DbSet<City> cities { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Profile> Profiles { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Brand>().HasData(
                new Brand { Brand_Id = 1, Brand_Name = "adidas"},
                new Brand { Brand_Id = 2, Brand_Name = "NIKE" },
                new Brand { Brand_Id = 3, Brand_Name = "Active" },
                new Brand { Brand_Id = 4, Brand_Name = "ZARA" },
                new Brand { Brand_Id = 5, Brand_Name = "H.M" }
                );

            modelBuilder.Entity<Category>().HasData(
                new Category {Category_id = 1, Category_Name = "T-Shirt" },
                new Category {Category_id = 2, Category_Name = "Pantalon" },
                new Category {Category_id = 3, Category_Name = "Shorts" }
                );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Product_Id = 1,
                    Product_Name = "Black Short",
                    Product_Details = "Black Short",
                    Product_Price = 69.00m,
                    Brand_Id = 2,
                    Category_id = 3,
                    imgUrl = "test.jpg"
                },
                new Product
                {
                    Product_Id = 2,
                    Product_Name = "Black T-shirt",
                    Product_Details = "Black T_Shirt",
                    Product_Price = 54.00m,
                    Brand_Id = 5 ,
                    Category_id = 1,
                    imgUrl = "test1.jpg"

                });

            modelBuilder.Entity<State>().HasData(
                
                new State
                {
                    Id = 1,
                    Name = "California"
                },

                new State
                {
                    Id = 2,
                    Name = "Florida"
                }
                );

            modelBuilder.Entity<City>().HasData(

                new City
                {
                    Id = 1,
                    name = "Arcadia"
                },

                new City
                {
                    Id = 2,
                    name = "Brea"
                },

                new City
                {
                    Id = 3,
                    name = "Chico"
                },
                new City
                {
                    Id = 4,
                    name = "Ajo"
                },
                new City
                {
                    Id = 5,
                    name = "Clifton"
                }
                );
        }
    }
}
