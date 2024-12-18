using IceCreamProject.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace IceCreamProject.ViewModels
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, ErrorMessage = "Category name must not exceed 100 characters.")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description must not exceed 500 characters.")]
        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public int? ParentCategoryId { get; set; }

        public IEnumerable<SelectListItem>? ParentCategories { get; set; }
    }

}
