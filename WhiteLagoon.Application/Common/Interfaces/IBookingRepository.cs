using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Common.Interfaces
{
    public interface IBookingRepository : IRepository<Booking>
    {
        public void Update(Booking booking);

        public void UpdateStatus(int bookingId, string bookingStatus, int villaNumber);

        public void UpdateStripePaymentID(int bookingId, string sessionId, string paymentIntentId);
    }
}