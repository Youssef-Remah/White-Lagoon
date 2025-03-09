using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Web.Controllers
{
    [Route("[controller]")]
    public class VillaNumberController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public VillaNumberController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Index()
        {
            var villaNumbers = await _unitOfWork.VillaNumber.GetAll(includeNavigationProperties: "Villa");

            return View(villaNumbers);
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
        public async Task<IActionResult> Create(VillaNumber villaNumber)
        {
            var villaNumbers = await _unitOfWork.VillaNumber.GetAll();

            bool isRoomNumberExists = villaNumbers.Any(vn => vn.Villa_Number == villaNumber.Villa_Number);

            if (ModelState.IsValid)
            {
                if (!isRoomNumberExists)
                {
                    await _unitOfWork.VillaNumber.Add(villaNumber);

                    await _unitOfWork.Save();

                    TempData["success"] = "The villa number has been created successfully.";

                    return RedirectToAction(nameof(Index));
                }

                else
                {
                    TempData["error"] = "The villa number already exists";
                }
            }

            return View();
        }


        [HttpGet]
        [Route("[action]/{VillaNumberId}")]
        public async Task<IActionResult> Update(int VillaNumberId)
        {
            var villas = await _unitOfWork.Villa.GetAll();

            IEnumerable<SelectListItem> listItems = villas.Select(v => new SelectListItem()
            {
                Text = v.Name,
                Value = v.Id.ToString()
            });

            ViewBag.VillaList = listItems;

            VillaNumber? villaNumberObject = await _unitOfWork.VillaNumber
                                                              .Get(vn => vn.Villa_Number == VillaNumberId);


            if (villaNumberObject is null)
            {
                string controllerName = nameof(HomeController);

                return RedirectToAction(nameof(HomeController.Error),
                    controllerName.Substring(0, controllerName.IndexOf("Controller")));
            }
            
            return View(villaNumberObject);
        }


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Update(VillaNumber villaNumber)
        {
            var villaNumbers = await _unitOfWork.VillaNumber.GetAll();

            bool isVillaNumberExists = villaNumbers.Any(vn => vn.Villa_Number == villaNumber.Villa_Number);

            if (ModelState.IsValid && isVillaNumberExists)
            {
                _unitOfWork.VillaNumber.Update(villaNumber);

                await _unitOfWork.Save();

                TempData["success"] = "Villa number updated successfully";

                return RedirectToAction(nameof(Index));
            }

            TempData["error"] = "Villa number could not be updated";

            return View(villaNumber);
        }


        [HttpGet]
        [Route("[action]/{VillaNumberId}")]
        public async Task<IActionResult> Delete(int VillaNumberId)
        {
            VillaNumber? villaNumber = await _unitOfWork.VillaNumber
                                                        .Get(vn => vn.Villa_Number == VillaNumberId);


            if (villaNumber is not null)
            {
                IEnumerable<Villa> villaItems = await _unitOfWork.Villa.GetAll();
                
                ViewBag.VillaName = villaItems.Where(i => i.Id == villaNumber.VillaId)
                                             .ToList()[0].Name;

                return View(villaNumber);
            }

            string controllerName = nameof(HomeController);

            return RedirectToAction(nameof(HomeController.Error),
                controllerName.Substring(0, controllerName.IndexOf("Controller")));
        }


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Delete(VillaNumber villaNumber)
        {
            VillaNumber? villaNumberToDelete = await _unitOfWork.VillaNumber
                                                                .Get(vn => vn.Villa_Number == villaNumber.Villa_Number);

            if (villaNumberToDelete is not null)
            {
                _unitOfWork.VillaNumber.Delete(villaNumberToDelete);

                await _unitOfWork.Save();

                TempData["success"] = "Villa Number deleted successfully";

                return RedirectToAction(nameof(Index));
            }

            string controllerName = nameof(HomeController);

            return RedirectToAction(nameof(HomeController.Error),
                controllerName.Substring(0, controllerName.IndexOf("Controller")));
        }
    }
}