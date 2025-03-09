using System.Linq.Expressions;

namespace WhiteLagoon.Application.Common.Interfaces
{
    public interface IRepository<T> where T : class
    {
        public Task<IEnumerable<T>> GetAllEntities(Expression<Func<T, bool>>? filter = null, string? includeNavigationProperties = null);

        public Task<T?> GetSingleEntity(Expression<Func<T, bool>> filter, string? includeNavigationProperties = null);

        public Task AddNewEntity(T newEntity);

        public Task DeleteEntity(T entity);
    }
}