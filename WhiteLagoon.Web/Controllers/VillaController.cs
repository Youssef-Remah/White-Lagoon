﻿using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Application.Common.Interfaces;

namespace WhiteLagoon.Web.Controllers
{
    [Route("[controller]")]
    public class VillaController : Controller
    {
        private readonly IVillaRepository _villaRepository;

        public VillaController(IVillaRepository villaRepository)
        {
            _villaRepository = villaRepository;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Index()
        {
             var villas = await _villaRepository.GetAllVillas();

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
                await _villaRepository.AddNewVilla(newVilla);

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
            Villa? villa = await _villaRepository.GetSingleVilla(v => v.Id == Id);

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
                await _villaRepository.UpdateVilla(villa);

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
            Villa? villa = await _villaRepository.GetSingleVilla(v => v.Id == Id);

            return View(villa);
        }


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Delete(Villa villa)
        {
            Villa? villaToDelete = await _villaRepository.GetSingleVilla(v => v.Id == villa.Id);

            if (villaToDelete is not null)
            {
                await _villaRepository.DeleteVilla(villaToDelete);

                TempData["success"] = "The Villa Has Been Deleted Successfully";

                return RedirectToAction(nameof(Index));
            }

            TempData["error"] = "The Villa Could Not Be Deleted";

            return View();
        }
    }
}