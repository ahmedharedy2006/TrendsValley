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
    public class StateRepo : Repo<State>, IStateRepo
    {
        private readonly AppDbContext _db;
        public StateRepo(AppDbContext db) : base(db)
        {
            _db = db;
        }
        public async Task UpdateAsync(State state)
        {
            _db.states.Update(state);
            await _db.SaveChangesAsync();
        }
    }
}
