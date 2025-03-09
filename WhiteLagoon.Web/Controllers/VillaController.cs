using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Application.Common.Interfaces;

namespace WhiteLagoon.Web.Controllers
{
    [Route("[controller]")]
    public class VillaController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public VillaController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Index()
        {
            var villas = await _unitOfWork.Villa.GetAll();

            return View(villas);
        }


        [HttpGet]
        [Route("[action]")]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Create(Villa newVilla)
        {
            if (newVilla.Description == newVilla.Name)
            {
                ModelState.AddModelError("Description", "The description cannot exactly match the name");
            }

            if (ModelState.IsValid)
            {
                await _unitOfWork.Villa.Add(newVilla);

                await _unitOfWork.Save();

                TempData["success"] = "The Villa Has Been Created Successfully";

                return RedirectToAction(nameof(Index));
            }

            TempData["success"] = "The Villa Could Not Be Added";

            return View();
        }


        [HttpGet]
        [Route("[action]/{Id}")]
        public async Task<IActionResult> Update(int Id)
        {
            Villa? villa = await _unitOfWork.Villa.Get(v => v.Id == Id);

            if (villa is null)
            {
                string controllerName = nameof(HomeController);

                return RedirectToAction(nameof(HomeController.Error),
                    controllerName.Substring(0, controllerName.IndexOf("Controller")));
            }

            return View(villa);
        }


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Update(Villa villa)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Villa.Update(villa);

                await _unitOfWork.Save();

                TempData["success"] = "The Villa Has Been Updated Successfully";

                return RedirectToAction(nameof(Index));
            }

            TempData["error"] = "The Villa Could Not Be Updated";

            return View(villa);
        }


        [HttpGet]
        [Route("[action]/{Id}")]
        public async Task<IActionResult> Delete(int Id)
        {
            Villa? villa = await _unitOfWork.Villa.Get(v => v.Id == Id);

            return View(villa);
        }


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Delete(Villa villa)
        {
            Villa? villaToDelete = await _unitOfWork.Villa.Get(v => v.Id == villa.Id);

            if (villaToDelete is not null)
            {
                _unitOfWork.Villa.Delete(villaToDelete);

                await _unitOfWork.Save();

                TempData["success"] = "The Villa Has Been Deleted Successfully";

                return RedirectToAction(nameof(Index));
            }

            TempData["error"] = "The Villa Could Not Be Deleted";

            return View();
        }
    }
}