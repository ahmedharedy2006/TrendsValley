using BooksMine.Models;
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
    public class CityRepo : Repo<City>, ICityRepo
    {
        private readonly AppDbContext _db;
        public CityRepo(AppDbContext db) : base(db)
        {
            _db = db;
        }
        public async Task UpdateAsync(City city)
        {

            _db.cities.Update(city);
            await _db.SaveChangesAsync();

        }
    }
    
}
