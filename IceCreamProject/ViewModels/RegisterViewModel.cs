using System.ComponentModel.DataAnnotations;

namespace IceCreamProject.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(100, ErrorMessage = "Username must be at least {2} characters long.", MinimumLength = 3)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, ErrorMessage = "Password must be at least {2} characters long.", MinimumLength = 6)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Delivery address is required.")]
        public string? Address { get; set; }

        public string? ProfileImageUrl { get; set; }
    }
}
