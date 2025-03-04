using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    }
}