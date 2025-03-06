using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Web.Controllers
{
    [Route("[controller]")]
    public class VillaNumberController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public VillaNumberController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Index()
        {
             var villaNumbers = await _dbContext.VillaNumbers
                                                .Include(vn => vn.Villa)
                                                .ToListAsync();

            return View(villaNumbers);
        }


        [HttpGet]
        [Route("[action]")]
        public IActionResult Create()
        {
            IEnumerable<SelectListItem> listItems = _dbContext.Villas
                                                       .ToList()
                                                       .Select(v => new SelectListItem()
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
            bool isRoomNumberExists = await _dbContext.VillaNumbers
                                                .AnyAsync(vn => vn.Villa_Number == villaNumber.Villa_Number);

            if (ModelState.IsValid)
            {
                if (!isRoomNumberExists)
                {
                    await _dbContext.VillaNumbers.AddAsync(villaNumber);

                    await _dbContext.SaveChangesAsync();

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
            IEnumerable<SelectListItem> listItems = _dbContext.Villas
                                           .ToList()
                                           .Select(v => new SelectListItem()
                                           {
                                               Text = v.Name,
                                               Value = v.Id.ToString()
                                           });

            ViewBag.VillaList = listItems;

            VillaNumber? villaNumberObject = await _dbContext.VillaNumbers
                                                       .FirstOrDefaultAsync(
                                                        vn => vn.Villa_Number == VillaNumberId);
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
            bool isVillaNumberExists = await _dbContext.VillaNumbers
                                                 .AnyAsync(
                                                 vn => vn.Villa_Number == villaNumber.Villa_Number);

            if (ModelState.IsValid && isVillaNumberExists)
            {
                _dbContext.VillaNumbers.Update(villaNumber);

                await _dbContext.SaveChangesAsync();

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
            VillaNumber? villaNumber = await _dbContext.VillaNumbers
                                                       .FirstOrDefaultAsync(
                                                       vn => vn.Villa_Number == VillaNumberId);


            if (villaNumber is not null)
            {
                List<Villa> villaItems = _dbContext.Villas.ToList();


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
            VillaNumber? villaNumberToDelete = await _dbContext.VillaNumbers
                                                       .FirstOrDefaultAsync(
                                                       vn => vn.Villa_Number == villaNumber.Villa_Number);


            if (villaNumberToDelete is not null)
            {
                _dbContext.VillaNumbers.Remove(villaNumberToDelete);

                await _dbContext.SaveChangesAsync();

                TempData["success"] = "Villa Number deleted successfully";

                return RedirectToAction(nameof(Index));
            }

            string controllerName = nameof(HomeController);

            return RedirectToAction(nameof(HomeController.Error),
                controllerName.Substring(0, controllerName.IndexOf("Controller")));
        }
    }
}