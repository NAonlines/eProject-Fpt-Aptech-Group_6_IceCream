using System.ComponentModel.DataAnnotations;

namespace IceCreamProject.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(20, ErrorMessage = "Username must be at least {2} characters long.", MinimumLength = 3)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email format is not valid.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, ErrorMessage = "Password must be at least {2} characters long.", MinimumLength = 6)]
        [RegularExpression(@"^(?=.*[A-Z]).{6,}$", ErrorMessage = "Password must contain at least one uppercase letter.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Delivery address is required.")]
        public string? Address { get; set; }

        [Phone(ErrorMessage = "Invalid phone number.")]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "Phone number must be at least 10 digits and no more than 15 digits.")]
        public string? PhoneNumber { get; set; }

        public string? ProfileImageUrl { get; set; }
    }
}
