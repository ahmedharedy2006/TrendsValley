using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendsValley.DataAccess.Repository.Interfaces
{
    public interface IUnitOfWork
    {
        IStateRepo StateRepo { get; }
        IBrandRepo BrandRepo { get; }
        IProductRepo ProductRepo { get; }
        ICategoryRepo CategoryRepo { get; }
        ICityRepo CityRepo { get; }

        IShoppingCartRepo ShoppingCartRepo { get; }

        IAppUserRepo AppUserRepo { get; }

        IOrderDetailsRepo orderDetailsRepo { get; }

        IOrderHeaderRepo orderHeaderRepo { get; }
        Task SaveAsync();
    }
}
