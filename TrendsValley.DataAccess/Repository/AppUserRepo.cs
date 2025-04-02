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
    public class AppUserRepo : Repo<AppUser> , IAppUserRepo
    {
        private readonly AppDbContext _db;
        public AppUserRepo(AppDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
