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
        public IActionResult Index()
        {
            return View();
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

            var villaNumbersList = (await _unitOfWork.VillaNumber.GetAll()).ToList();

            var bookedVillas = (await _unitOfWork.Booking
                                          .GetAll(u => u.Status == SD.StatusApproved 
                                          || u.Status == SD.StatusCheckedIn)).ToList();

            int roomAvailable = SD.VillaRoomsAvailable_Count(villa.Id, villaNumbersList, booking.CheckInDate, booking.Nights, bookedVillas);

            if (roomAvailable == 0)
            {
                TempData["error"] = "Room has been sold out!";

                return RedirectToAction(nameof(FinalizeBooking), new
                {
                    villaId = booking.VillaId,
                    checkInDate = booking.CheckInDate,

                    nights = booking.Nights
                });
            }

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
                    _unitOfWork.Booking.UpdateStatus(bookingFromDb.Id, SD.StatusApproved, 0);

                    _unitOfWork.Booking.UpdateStripePaymentID(bookingFromDb.Id, session.Id, session.PaymentIntentId);

                    await _unitOfWork.Save();
                }
            }

            return View(bookingId);
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> BookingDetails(int bookingId)
        {
            Booking bookingFromDb = await _unitOfWork.Booking
                                         .Get(b => b.Id == bookingId,
                                         includeNavigationProperties: "User,Villa");


            if (bookingFromDb.VillaNumber == 0 && bookingFromDb.Status == SD.StatusApproved)
            {
                var availableVillaNumber = await AssignAvailableVillaNumberByVilla(bookingFromDb.VillaId);

                var villaNumbers = await _unitOfWork.VillaNumber
                                                        .GetAll(u => u.VillaId == bookingFromDb.VillaId &&
                                                        availableVillaNumber.Any(x => x == u.Villa_Number));
                
                bookingFromDb.VillaNumbers = villaNumbers.ToList();
            }

            return View(bookingFromDb);
        }

        private async Task<List<int>> AssignAvailableVillaNumberByVilla(int villaId)
        {
            List<int> availableVillaNumbers = new();

            var villaNumbers = await _unitOfWork.VillaNumber.GetAll(u => u.VillaId == villaId);

            var villas = await _unitOfWork.Booking
                                            .GetAll(u => u.VillaId == villaId && u.Status == SD.StatusCheckedIn);

            var checkedInVilla = villas.Select(u => u.VillaNumber);

            foreach (var villaNumber in villaNumbers)
            {
                if (!checkedInVilla.Contains(villaNumber.Villa_Number))
                {
                    availableVillaNumbers.Add(villaNumber.Villa_Number);
                }
            }

            return availableVillaNumbers;
        }


        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        [Route("[action]")]
        public IActionResult CheckIn(Booking booking)
        {
            _unitOfWork.Booking.UpdateStatus(booking.Id, SD.StatusCheckedIn, booking.VillaNumber);

            _unitOfWork.Save();

            TempData["Success"] = "Booking Updated Successfully.";

            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
        }


        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        [Route("[action]")]
        public IActionResult CheckOut(Booking booking)
        {
            _unitOfWork.Booking.UpdateStatus(booking.Id, SD.StatusCompleted, booking.VillaNumber);

            _unitOfWork.Save();

            TempData["Success"] = "Booking Completed Successfully.";

            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
        }


        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        [Route("[action]")]
        public IActionResult CancelBooking(Booking booking)
        {
            _unitOfWork.Booking.UpdateStatus(booking.Id, SD.StatusCancelled, 0);

            _unitOfWork.Save();

            TempData["Success"] = "Booking Cancelled Successfully.";

            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
        }


        #region API Calls

        [HttpGet]
        [Route("[action]")]
        [Authorize]
        public async Task<IActionResult> GetAll(string status)
        {
            IEnumerable<Booking> bookings;

            if (User.IsInRole(SD.Role_Admin))
            {
                bookings = await _unitOfWork.Booking
                                            .GetAll(includeNavigationProperties: "User,Villa");
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;

                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                bookings = await _unitOfWork.Booking.GetAll(b => b.UserId == userId,
                includeNavigationProperties: "User,Villa");
            }
            if (!string.IsNullOrEmpty(status))
            {
                bookings = bookings.Where(b => b.Status.ToLower() == status.ToLower());
            }

            return Json(new { data = bookings });
        }

        #endregion
    }
}