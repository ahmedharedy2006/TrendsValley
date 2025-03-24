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
    public class ShoppingCartRepo : Repo<ShoppingCart>, IShoppingCartRepo
    {
        private readonly AppDbContext _db;
        public ShoppingCartRepo(AppDbContext db) : base(db)
        {
            _db = db;
        }
        public async Task UpdateAsync(ShoppingCart shoppingCart)
        {
            _db.Carts.Update(shoppingCart);
            await _db.SaveChangesAsync();
        }
    }
}
