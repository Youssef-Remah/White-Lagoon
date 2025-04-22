using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Web.Controllers
{
    public class BookingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<IActionResult> FinalizeBooking(int villaId, DateOnly checkInDate, int nights)
        {
            Booking booking = new()
            {
                VillaId = villaId,
                Villa = await _unitOfWork.Villa
                                         .Get(v => v.Id == villaId,
                                         includeNavigationProperties: "VillaAmenities"),
                CheckInDate = checkInDate,
                Nights = nights,
                CheckOutDate = checkInDate.AddDays(nights),
            };

            return View(booking);
        }
    }
}