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
    public class ProductRepo :Repo<Product> ,IProductRepo
    {
        private readonly AppDbContext _db;
        public ProductRepo(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task UpdateAsync(Product product)
        {
            _db.Products.Update(product);
            await _db.SaveChangesAsync();
        }
    }
   
}
