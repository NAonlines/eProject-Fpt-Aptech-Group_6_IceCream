using System.ComponentModel.DataAnnotations;

namespace IceCreamProject.ViewModels
{
    public class FeedbackViewModel
    {
        [Required(ErrorMessage = "Your name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Subject is required.")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Message is required.")]
        [StringLength(500, ErrorMessage = "Message must be between 10 and 500 characters.", MinimumLength = 10)]
        public string Message { get; set; }
    }
}
