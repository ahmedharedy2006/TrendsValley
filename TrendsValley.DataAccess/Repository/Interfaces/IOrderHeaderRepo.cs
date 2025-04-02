using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrendsValley.Models.Models;

namespace TrendsValley.DataAccess.Repository.Interfaces
{
    public interface IOrderHeaderRepo : IRepo<OrderHeader>
    {
        Task UpdateAsync(OrderHeader orderHeader);

        Task UpdateStatus(int id, string Orderstatus, string? paymentStatus = null);

        Task UpdateStripePaymentIntentId(int id, string paymentIntentId, string sessionId);
    }
}
