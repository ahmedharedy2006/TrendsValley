using Microsoft.EntityFrameworkCore;
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
    public class OrderHeaderRepo : Repo<OrderHeader> , IOrderHeaderRepo
    {
        private readonly AppDbContext _db;

        public OrderHeaderRepo(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public Task UpdateAsync(OrderHeader orderHeader)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateStatus(int id, string Orderstatus, string? paymentStatus = null)
        {
            var orderFromDb = await _db.OrderHeaders.FirstOrDefaultAsync(o => o.Id == id);
            if (orderFromDb != null)
            {
                orderFromDb.orderStatus = Orderstatus;
                if (paymentStatus != null)
                {
                    orderFromDb.paymentStatus = paymentStatus;
                }
            }
        }

        public async Task UpdateStripePaymentIntentId(int id, string paymentIntentId, string sessionId)
        {
            var orderFromDb = await _db.OrderHeaders.FirstOrDefaultAsync(o => o.Id == id);
            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                orderFromDb.paymentIntentId = paymentIntentId;
                orderFromDb.paymentDate = DateTime.Now;
            }

            if (!string.IsNullOrEmpty(sessionId))
            {
                orderFromDb.sessionId = sessionId;
            }
        }
    }
}
