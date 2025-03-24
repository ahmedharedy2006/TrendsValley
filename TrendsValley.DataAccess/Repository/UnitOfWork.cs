using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrendsValley.DataAccess.Data;
using TrendsValley.DataAccess.Repository.Interfaces;

namespace TrendsValley.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;

        public IBrandRepo BrandRepo { get; private set; }
        public ICategoryRepo CategoryRepo { get; private set; }
        public IProductRepo ProductRepo { get; private set; }
        public IStateRepo StateRepo { get; private set; }
        public ICityRepo CityRepo { get; private set; }

        public IShoppingCartRepo ShoppingCartRepo { get; private set; }
        public UnitOfWork(AppDbContext db)
        {
            _db = db;
            BrandRepo = new BrandRepo(db);
            CategoryRepo = new CategoryRepo(db);
            ProductRepo = new ProductRepo(db);
            StateRepo = new StateRepo(db);
            CityRepo = new CityRepo(db);
            ShoppingCartRepo = new ShoppingCartRepo(db);
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
