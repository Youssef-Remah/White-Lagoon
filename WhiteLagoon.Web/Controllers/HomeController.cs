using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    [Route("[controller]")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;


        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpGet]
        [Route("/")]
        [Route("[action]")]
        public async Task<IActionResult> Index()
        {
            var villaList = await _unitOfWork.Villa.GetAll(includeNavigationProperties: "VillaAmenities");

            HomeViewModel homeVM = new()
            {
                VillaList = villaList,

                NumberOfNights = 1,

                CheckInDate = DateOnly.FromDateTime(DateTime.Now)
            };

            return View(homeVM);
        }


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Index(HomeViewModel homeViewModel)
        {
            homeViewModel.VillaList = await _unitOfWork.Villa
                                                       .GetAll(includeNavigationProperties: "VillaAmenities");

            return View(homeViewModel);
        }


        public async Task<IActionResult> GetVillasByDate(int nights, DateOnly checkInDate)
        {
            var villaList = await _unitOfWork.Villa
                                             .GetAll(includeNavigationProperties: "VillaAmenities");

            foreach (var villa in villaList)
            {
                if (villa.Id % 2 == 0)
                {
                    villa.IsAvailable = false;
                }
            }

            HomeViewModel homeViewModel = new()
            {
                CheckInDate = checkInDate,
                VillaList = villaList,
                NumberOfNights = nights
            };

            return PartialView("_VillaList", homeViewModel);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Error()
        {
            return View();
        }
    }
}
