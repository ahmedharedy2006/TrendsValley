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

        public DbSet<Customer> customers { get; set; }
        public DbSet<AppUser> appUsers { get; set; }

        public DbSet<State> states { get; set; }

        public DbSet<City> cities { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Profile> Profiles { get; set; }

        public DbSet<ShoppingCart> Carts { get; set; }

        public DbSet<OrderHeader> OrderHeaders { get; set; }

        public DbSet<OrderDetails> OrderDetails { get; set; }

        public DbSet<UserDevice> UserDevices { get; set; }
        public DbSet<SecurityActivity> SecurityActivities { get; set; }
        public DbSet<AdminActivity> AdminActivities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Brand>().HasData(
                new Brand { Brand_Id = 1, Brand_Name = "adidass" },
                new Brand { Brand_Id = 2, Brand_Name = "NIKEe" },
                new Brand { Brand_Id = 3, Brand_Name = "Activee" },
                new Brand { Brand_Id = 4, Brand_Name = "ZARAa" },
                new Brand { Brand_Id = 5, Brand_Name = "H.Ms" }
                );

            modelBuilder.Entity<Category>().HasData(
                new Category { Category_id = 1, Category_Name = "T-Shirts" },
                new Category { Category_id = 2, Category_Name = "Pantalons" },
                new Category { Category_id = 3, Category_Name = "Shortss" }
                );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Product_Id = 1,
                    Product_Name = "Black Shortssss",
                    Product_Details = "Black Shortss",
                    Product_Price = 69.00m,
                    Brand_Id = 2,
                    Category_id = 3,
                    imgUrl = "test.jpg"
                },
                new Product
                {
                    Product_Id = 2,
                    Product_Name = "Black T-shirtsss",
                    Product_Details = "Black T_Shirtsss",
                    Product_Price = 54.00m,
                    Brand_Id = 5,
                    Category_id = 1,
                    imgUrl = "test1.jpg"

                });

            modelBuilder.Entity<State>().HasData(

                new State
                {
                    Id = 1,
                    Name = "Californias"
                },

                new State
                {
                    Id = 2,
                    Name = "Floridas"
                }
                );

            modelBuilder.Entity<City>().HasData(

                new City
                {
                    Id = 1,
                    name = "Arcadias"
                },

                new City
                {
                    Id = 2,
                    name = "Breas"
                },

                new City
                {
                    Id = 3,
                    name = "Chicos"
                },
                new City
                {
                    Id = 4,
                    name = "Ajos"
                },
                new City
                {
                    Id = 5,
                    name = "Cliftons"
                }
                );
        }
    }
}
