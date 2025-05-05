using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Infrastructure.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _dbContext;

        internal DbSet<T> _dbSet;

        public Repository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;

            _dbSet = _dbContext.Set<T>();
        }

        public async Task Add(T newEntity)
        {
            await _dbSet.AddAsync(newEntity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<IEnumerable<T>> GetAll(
            Expression<Func<T, bool>>? filter = null,
            string? includeNavigationProperties = null,
            bool tracked = false
        )
        {
            IQueryable<T> query;

            if (tracked)
            {
                query = _dbSet;
            }
            else
            {
                query = _dbSet.AsNoTracking();
            }

            if (filter is not null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(includeNavigationProperties))
            {
                var properties = includeNavigationProperties.Split(',', StringSplitOptions.RemoveEmptyEntries);

                foreach (string prop in properties)
                {
                    query = query.Include(prop);
                }
            }

            return await query.ToListAsync();
        }

        public async Task<T?> Get(
            Expression<Func<T, bool>> filter,
            string? includeNavigationProperties = null,
            bool tracked = false
        )
        {
            IQueryable<T> query;

            if (tracked)
            {
                query = _dbSet;
            }
            else
            {
                query = _dbSet.AsNoTracking();
            }

            if (filter is not null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(includeNavigationProperties))
            {
                var properties = includeNavigationProperties.Split(',', StringSplitOptions.RemoveEmptyEntries);

                foreach (string prop in properties)
                {
                    query = query.Include(prop);
                }
            }

            return await query.FirstOrDefaultAsync();
        }
    }
}