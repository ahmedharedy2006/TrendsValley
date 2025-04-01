using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrendsValley.Models.Models;

namespace TrendsValley.DataAccess.Repository.Interfaces
{
    public interface IOrderDetailsRepo : IRepo<OrderDetails>
    {
        void UpdateAsync(OrderDetails orderDetails);
    }
}
