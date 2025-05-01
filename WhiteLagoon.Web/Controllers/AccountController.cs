using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
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
        public IActionResult Login(string? ReturnUrl)
        {
            ReturnUrl ??= Url.Content("~/");

            LoginViewModel loginViewModel = new()
            {
                RedirectUrl = ReturnUrl
            };

            return View(loginViewModel);
        }


        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Register(string? ReturnUrl)
        {
            ReturnUrl ??= Url.Content("~/");

            if (!await _roleManager.RoleExistsAsync(SD.Role_Admin))
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));

                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer));
            }

            RegisterViewModel registerViewModel = new()
            {
                RoleList = _roleManager.Roles.Select(role => new SelectListItem()
                {
                    Text = role.Name,

                    Value = role.Name
                }),

                RedirectUrl = ReturnUrl
            };

            return View(registerViewModel);
        }


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new()
                {
                    Name = registerViewModel.Name,

                    Email = registerViewModel.Email,

                    PhoneNumber = registerViewModel.PhoneNumber,

                    NormalizedEmail = registerViewModel.Email.ToUpper(),

                    EmailConfirmed = true,

                    UserName = registerViewModel.Email,

                    CreatedAt = DateTime.Now
                };

                var result = await _userManager.CreateAsync(user, registerViewModel.Password);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(registerViewModel.Role))
                    {
                        await _userManager.AddToRoleAsync(user, registerViewModel.Role);
                    }

                    else
                    {
                        await _userManager.AddToRoleAsync(user, SD.Role_Customer);
                    }

                    await _signInManager.SignInAsync(user, false);

                    if (string.IsNullOrEmpty(registerViewModel.RedirectUrl))
                    {
                        return RedirectToAction(nameof(HomeController.Index), "Home");
                    }

                    else
                    {
                        return LocalRedirect(registerViewModel.RedirectUrl);
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            registerViewModel.RoleList = _roleManager.Roles.Select(role => new SelectListItem()
            {
                Text = role.Name,
                Value = role.Name
            });

            return View(registerViewModel);
        }


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    loginViewModel.Email,
                    loginViewModel.Password,
                    loginViewModel.RememberMe,
                    lockoutOnFailure: false
                );

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(loginViewModel.Email);

                    if (await _userManager.IsInRoleAsync(user, SD.Role_Admin))
                    {
                        return RedirectToAction(nameof(DashboardController.Index), "Dashboard");
                    }

                    else 
                    {
                        if (string.IsNullOrEmpty(loginViewModel.RedirectUrl))
                        {
                            return RedirectToAction(nameof(HomeController.Index), "Home");
                        }

                        else
                        {
                            return LocalRedirect(loginViewModel.RedirectUrl);
                        }
                    }
                }

                else
                {
                    ModelState.AddModelError("", "Invalid Login Attempt");
                }
            }

            return View(loginViewModel);
        }


        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Logout()
        {
           await _signInManager.SignOutAsync();

           return RedirectToAction(nameof(HomeController.Index), "Home");
        }


        [HttpGet]
        [Route("[action]")]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}