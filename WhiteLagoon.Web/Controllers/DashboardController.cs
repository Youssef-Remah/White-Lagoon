using Microsoft.AspNetCore.Mvc;

namespace WhiteLagoon.Web.Controllers
{
    [Route("[controller]")]
    public class DashboardController : Controller
    {
        [HttpGet]
        [Route("[action]")]
        public IActionResult Index()
        {
            return View();
        }
    }
}