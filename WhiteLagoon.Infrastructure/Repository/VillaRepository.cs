using System.Linq.Expressions;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Infrastructure.Repository
{
    public class VillaRepository : IVillaRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public VillaRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddNewVilla(Villa newVilla)
        {
            await _dbContext.Villas.AddAsync(newVilla);

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteVilla(Villa villa)
        {
            _dbContext.Villas.Remove(villa);

            await _dbContext.SaveChangesAsync();
        }

        public Task<IEnumerable<Villa>> GetAllVillas(Expression<Func<Villa, bool>>? filter = null, string? includeNavigationProperties = null)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Villa>> GetSingleVilla(Expression<Func<Villa, bool>> filter, string? includeNavigationProperties = null)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateVilla(Villa villa)
        {
            _dbContext.Villas.Update(villa);

            await _dbContext.SaveChangesAsync();
        }
    }
}