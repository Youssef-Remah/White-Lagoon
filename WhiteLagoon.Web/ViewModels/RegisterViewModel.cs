using System.ComponentModel.DataAnnotations;

namespace WhiteLagoon.Web.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string Name { get; set; }


        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }


        [Required]
        public string Email { get; set; }


        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password does not match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }


        public string? RedirectUrl { get; set; }
    }
}