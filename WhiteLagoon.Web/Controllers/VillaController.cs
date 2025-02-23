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
            if (ModelState.IsValid)
            {
                await _dbContext.Villas.AddAsync(newVilla);

                await _dbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View();
        }
    }
}