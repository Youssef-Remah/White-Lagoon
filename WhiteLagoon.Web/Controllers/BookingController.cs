using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Web.Controllers
{
    [Route("[controller]")]
    public class BookingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> FinalizeBooking(int villaId, DateOnly checkInDate, int nights)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ApplicationUser user = await _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            Booking booking = new()
            {
                VillaId = villaId,
                Villa = await _unitOfWork.Villa
                                         .Get(v => v.Id == villaId,
                                         includeNavigationProperties: "VillaAmenities"),
                CheckInDate = checkInDate,
                Nights = nights,
                CheckOutDate = checkInDate.AddDays(nights),
                UserId = userId,
                Phone = user.PhoneNumber,
                Name = user.Name,
                Email = user.Email
            };

            booking.TotalCost = booking.Villa.Price * nights;

            return View(booking);
        }

        [Authorize]
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> FinalizeBooking(Booking booking)
        {
            var villa = await _unitOfWork.Villa.Get(v => v.Id == booking.VillaId);

            booking.TotalCost = villa.Price * booking.Nights;

            booking.Status = SD.StatusPending;

            booking.BookingDate = DateTime.Now;

            await _unitOfWork.Booking.Add(booking);

            await _unitOfWork.Save();

            var domain = Request.Scheme + "://" + Request.Host.Value + "/";

            var options = new SessionCreateOptions()
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"Booking/BookingConfirmation?bookingId={booking.Id}",
                CancelUrl = domain + $"Booking/FinalizeBooking?villaId={booking.VillaId}&checkInDate={booking.CheckInDate}&nights={booking.Nights}",
            };

            options.LineItems.Add(new SessionLineItemOptions()
            {
                PriceData = new SessionLineItemPriceDataOptions()
                {
                    UnitAmount = (long)(booking.TotalCost * 100),
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions()
                    {
                        Name = villa.Name,
                    },
                },
                Quantity = 1,
            });

            var service = new SessionService();

            Session session = service.Create(options);

            _unitOfWork.Booking.UpdateStripePaymentID(booking.Id, session.Id, session.PaymentIntentId);

            await _unitOfWork.Save();

            Response.Headers.Append("Location", session.Url);

            return StatusCode(303);
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> BookingConfirmation(int bookingId)
        {
            Booking bookingFromDb = await _unitOfWork.Booking
                                         .Get(b => b.Id == bookingId,
                                         includeNavigationProperties: "User,Villa");

            if (bookingFromDb.Status == SD.StatusPending)
            {
                var service = new SessionService();

                Session session = service.Get(bookingFromDb.StripeSessionId);

                if (session.PaymentStatus == "paid")
                {
                    _unitOfWork.Booking.UpdateStatus(bookingFromDb.Id, SD.StatusApproved);

                    _unitOfWork.Booking.UpdateStripePaymentID(bookingFromDb.Id, session.Id, session.PaymentIntentId);

                    await _unitOfWork.Save();
                }
            }

            return View(bookingId);
        }
    }
}