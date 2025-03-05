using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Web.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public VillaNumberController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
             var villaNumbers = await _dbContext.VillaNumbers.ToListAsync();

            return View(villaNumbers);
        }


        [HttpGet]
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
        public async Task<IActionResult> Create(VillaNumber villaNumber)
        {
            if (ModelState.IsValid)
            {
                await _dbContext.VillaNumbers.AddAsync(villaNumber);

                await _dbContext.SaveChangesAsync();

                TempData["success"] = "The villa number has been created successfully.";

                return RedirectToAction("Index");
            }

            return View();
        }
    }
}