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
    public class OrderDetailsRepo : Repo<OrderDetails> , IOrderDetailsRepo
    {
        private readonly AppDbContext _db;

        public OrderDetailsRepo(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public void UpdateAsync(OrderDetails orderDetails)
        {
            _db.OrderDetails.Update(orderDetails);
        }
    }
}
