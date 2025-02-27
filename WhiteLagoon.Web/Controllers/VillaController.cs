using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Web.Controllers
{
    public class VillaController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public VillaController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
             var villas = await _dbContext.Villas.ToListAsync();

            return View(villas);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(Villa newVilla)
        {
            if (newVilla.Description == newVilla.Name)
            {
                ModelState.AddModelError("Description", "The description cannot exactly match the name");
            }

            if (ModelState.IsValid)
            {
                await _dbContext.Villas.AddAsync(newVilla);

                await _dbContext.SaveChangesAsync();

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
            Villa? villa = await _dbContext.Villas.FirstOrDefaultAsync(v => v.Id == Id);

            if (villa == null)
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
                _dbContext.Villas.Update(villa);

                await _dbContext.SaveChangesAsync();

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
            Villa? villa = await _dbContext.Villas.FirstOrDefaultAsync(v => v.Id == Id);

            return View(villa);
        }


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Delete(Villa villa)
        {
            Villa? villaToDelete = await _dbContext.Villas.FirstOrDefaultAsync(v => v.Id == villa.Id);

            if (villaToDelete != null)
            {
                _dbContext.Villas.Remove(villaToDelete);

                await _dbContext.SaveChangesAsync();

                TempData["success"] = "The Villa Has Been Deleted Successfully";

                return RedirectToAction(nameof(Index));
            }

            TempData["error"] = "The Villa Could Not Be Deleted";

            return View();
        }
    }
}