using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TrendsValley.DataAccess.Repository.Interfaces
{
    public interface IRepo<T> where T : class
    {
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null , Expression<Func<T, object>>[]? includeProperties = null);

        Task<T> GetAsync(Expression<Func<T, bool>> filter = null, bool tracked = true, Expression<Func<T, object>>[]? includeProperties = null);

        Task CreateAsync(T entity);

        Task RemoveAsync(T entity);

        Task RemoveRangeAsync(IEnumerable<T> entities);
        Task AdminActivityAsync(string userId, string activityType, string description, string ipAddress);


    }
}
