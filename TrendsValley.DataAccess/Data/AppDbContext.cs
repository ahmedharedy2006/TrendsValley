using BooksMine.Models;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
