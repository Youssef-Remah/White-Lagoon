using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index()
        {
             var villas = _dbContext.Villas.ToList();

            return View(villas);
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}