﻿using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public IVillaRepository Villa { get; private set; }

        public IVillaNumberRepository VillaNumber { get; private set; }

        public IAmenityRepository Amenity { get; private set; }

        private readonly ApplicationDbContext _dbContext;


        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;

            Villa = new VillaRepository(_dbContext);

            VillaNumber = new VillaNumberRepository(_dbContext);

            Amenity = new AmenityRepository(_dbContext);
        }

        public async Task Save()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}