using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Application.Common.Interfaces;

namespace WhiteLagoon.Web.Controllers
{
    [Route("[controller]")]
    public class VillaController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public VillaController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;

            _webHostEnvironment = webHostEnvironment;
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
                if (newVilla.Image is not null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(newVilla.Image.FileName);

                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"images\VillaImage");

                    using (var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
                    {
                        newVilla.Image.CopyTo(fileStream);
                    }

                    newVilla.ImageUrl = @"images\VillaImage\" + fileName;
                }
                else
                {
                    newVilla.ImageUrl = "https://placehold.co/600x400";
                }

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
                if (villa.Image is not null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(villa.Image.FileName);

                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"images\VillaImage");

                    if (!string.IsNullOrEmpty(villa.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath,
                            villa.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
                    {
                        villa.Image.CopyTo(fileStream);
                    }

                    villa.ImageUrl = @"images\VillaImage\" + fileName;
                }

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
                if (!string.IsNullOrEmpty(villaToDelete.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath,
                        villaToDelete.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

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