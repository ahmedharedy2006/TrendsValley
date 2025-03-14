using TrendsValley.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendsValley.DataAccess.Repository.Interfaces
{
    public interface ICityRepo : IRepo<City>
    {
        Task UpdateAsync(City city);
    }
}
