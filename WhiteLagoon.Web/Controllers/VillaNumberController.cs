using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
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

                    return RedirectToAction("Index");
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

            return View(villaNumberObject);
        }
    }
}