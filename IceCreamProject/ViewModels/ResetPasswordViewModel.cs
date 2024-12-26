using System.ComponentModel.DataAnnotations;

namespace IceCreamProject.ViewModels;
public class ResetPasswordViewModel
{
    [Required]
    public string Token { get; set; }

    [Required]
    public string UserId { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, ErrorMessage = "Password must be at least {2} characters long.", MinimumLength = 6)]
    [RegularExpression(@"^(?=.*[A-Z]).{6,}$", ErrorMessage = "Password must contain at least one uppercase letter.")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "Confirm password is required.")]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; }
}
