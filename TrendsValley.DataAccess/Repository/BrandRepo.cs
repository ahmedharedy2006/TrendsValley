using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrendsValley.DataAccess.Data;
using TrendsValley.DataAccess.Repository.Interfaces;
using TrendsValley.Models.Models;

namespace TrendsValley.DataAccess.Repository
{
    public class BrandRepo : Repo<Brand>, IBrandRepo
    {
        private readonly AppDbContext _db;
        public BrandRepo(AppDbContext db) : base(db)
        {
            _db = db;
        }
        public async Task UpdateAsync(Brand brand)
        {
            _db.Brands.Update(brand);
        }
    }
   
}
