using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IceCreamProject.ViewModels
{
    public class UserViewModel
    {
        public string UserId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please select a role.")]
        public int RoleId { get; set; }

        public bool IsActive { get; set; }

        public List<SelectListItem> Roles { get; set; }
    }
}
