using System.Linq.Expressions;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Common.Interfaces
{
    public interface IVillaRepository
    {
        public Task<IEnumerable<Villa>> GetAllVillas(Expression<Func<Villa, bool>>? filter = null, string? includeNavigationProperties = null);

        public Task<IEnumerable<Villa>> GetSingleVilla(Expression<Func<Villa, bool>> filter, string? includeNavigationProperties = null);

        public Task AddNewVilla(Villa newVilla);

        public Task UpdateVilla(Villa villa);

        public Task DeleteVilla(Villa villa);
    }
}