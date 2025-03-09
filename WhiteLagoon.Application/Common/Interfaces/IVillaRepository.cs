using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Common.Interfaces
{
    public interface IVillaRepository : IRepository<Villa>
    {
        public Task Update(Villa villa);
    }
}