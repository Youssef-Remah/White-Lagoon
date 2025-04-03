using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Web.Controllers
{
    [Route("[controller]")]
    [Authorize(Roles = SD.Role_Admin)]
    public class AmenityController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AmenityController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Index()
        {
            var amenities = await _unitOfWork.Amenity.GetAll(includeNavigationProperties: "Villa");

            return View(amenities);
        }


        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Create()
        {
            var villas = await _unitOfWork.Villa.GetAll();

            IEnumerable<SelectListItem> listItems = villas.Select(v => new SelectListItem()
            {
                Text = v.Name,
                Value = v.Id.ToString()
            });

            ViewBag.VillaList = listItems;

            return View();
        }


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Create(Amenity amenity)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Amenity.Add(amenity);

                await _unitOfWork.Save();

                TempData["success"] = "The amenity has been created successfully.";

                return RedirectToAction(nameof(Index));
            }

            return View();
        }


        [HttpGet]
        [Route("[action]/{AmenityId}")]
        public async Task<IActionResult> Update(int AmenityId)
        {
            var villas = await _unitOfWork.Villa.GetAll();

            IEnumerable<SelectListItem> listItems = villas.Select(v => new SelectListItem()
            {
                Text = v.Name,
                Value = v.Id.ToString()
            });

            ViewBag.VillaList = listItems;

            Amenity? amenityObject = await _unitOfWork.Amenity.Get(am => am.Id == AmenityId);


            if (amenityObject is null)
            {
                string controllerName = nameof(HomeController);

                return RedirectToAction(nameof(HomeController.Error),
                    controllerName.Substring(0, controllerName.IndexOf("Controller")));
            }

            return View(amenityObject);
        }


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Update(Amenity amenity)
        {
            var amenities = await _unitOfWork.Amenity.GetAll();

            bool isAmenityExists = amenities.Any(am => am.Id == amenity.Id);

            if (ModelState.IsValid && isAmenityExists)
            {
                _unitOfWork.Amenity.Update(amenity);

                await _unitOfWork.Save();

                TempData["success"] = "Amenity updated successfully";

                return RedirectToAction(nameof(Index));
            }

            TempData["error"] = "Amenity could not be updated";

            return View(amenity);
        }


        [HttpGet]
        [Route("[action]/{AmenityId}")]
        public async Task<IActionResult> Delete(int AmenityId)
        {
            Amenity? amenity = await _unitOfWork.Amenity.Get(am => am.Id == AmenityId);


            if (amenity is not null)
            {
                IEnumerable<Villa> villaItems = await _unitOfWork.Villa.GetAll();

                ViewBag.VillaName = villaItems.Where(v => v.Id == amenity.VillaId)
                                             .ToList()[0].Name;

                return View(amenity);
            }

            string controllerName = nameof(HomeController);

            return RedirectToAction(nameof(HomeController.Error),
                controllerName.Substring(0, controllerName.IndexOf("Controller")));
        }


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Delete(Amenity amenity)
        {
            Amenity? amenityToDelete = await _unitOfWork.Amenity.Get(am => am.Id == amenity.Id);

            if (amenityToDelete is not null)
            {
                _unitOfWork.Amenity.Delete(amenityToDelete);

                await _unitOfWork.Save();

                TempData["success"] = "Amenity deleted successfully";

                return RedirectToAction(nameof(Index));
            }

            string controllerName = nameof(HomeController);

            return RedirectToAction(nameof(HomeController.Error),
                controllerName.Substring(0, controllerName.IndexOf("Controller")));
        }
    }
}