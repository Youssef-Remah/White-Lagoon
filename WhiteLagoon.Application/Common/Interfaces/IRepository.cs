using System.Linq.Expressions;

namespace WhiteLagoon.Application.Common.Interfaces
{
    public interface IRepository<T> where T : class
    {
        public Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? filter = null, string? includeNavigationProperties = null, bool tracked = false);

        public Task<T?> Get(Expression<Func<T, bool>> filter, string? includeNavigationProperties = null, bool tracked = false);

        public Task Add(T newEntity);

        public void Delete(T entity);
    }
}