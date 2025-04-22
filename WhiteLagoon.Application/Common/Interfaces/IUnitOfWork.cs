namespace WhiteLagoon.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        public IVillaRepository Villa { get; }

        public IAmenityRepository Amenity { get; }

        public IVillaNumberRepository VillaNumber { get; }

        public IBookingRepository BookingRepository { get; }

        public IApplicationUserRepository ApplicationUser { get; }

        public Task Save();
    }
}