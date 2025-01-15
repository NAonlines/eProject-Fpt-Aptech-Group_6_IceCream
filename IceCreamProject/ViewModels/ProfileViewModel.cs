using System.ComponentModel.DataAnnotations; // For validation attributes
using IceCreamProject.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace IceCreamProject.ViewModels
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "Phone number must be at least 10 digits and no more than 15 digits.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(250, ErrorMessage = "Address must not exceed 250 characters.")]
        public string Address { get; set; }

        public string ProfileImageUrl { get; set; } 

        public IFormFile ProfileImage { get; set; }

        public List<Order> TransactionHistory { get; set; } = new List<Order>();
    }
}
