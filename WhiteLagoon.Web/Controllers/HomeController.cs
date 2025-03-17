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


        [Route("/")]
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


        [Route("[action]")]
        public IActionResult Privacy()
        {
            return View();
        }


        [Route("[action]")]
        public IActionResult Error()
        {
            return View();
        }
    }
}
