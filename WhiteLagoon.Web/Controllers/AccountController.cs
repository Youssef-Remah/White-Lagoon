using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly RoleManager<IdentityRole> _roleManager;


        public AccountController
        (
            IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager
        )
        {
            _unitOfWork = unitOfWork;

            _userManager = userManager;

            _signInManager = signInManager;

            _roleManager = roleManager;
        }


        [HttpGet]
        [Route("[action]")]
        public IActionResult Login(string? redirectUrl)
        {
            redirectUrl ??= Url.Content("~/");

            LoginViewModel loginViewModel = new()
            {
                RedirectUrl = redirectUrl
            };

            return View(loginViewModel);
        }

        public async Task<IActionResult> Register()
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));

                await _roleManager.CreateAsync(new IdentityRole("Customer"));
            }

            RegisterViewModel registerViewModel = new()
            {
                RoleList = _roleManager.Roles.Select(role => new SelectListItem()
                {
                    Text = role.Name,

                    Value = role.Name
                })
            };

            return View(registerViewModel);
        }
    }
}