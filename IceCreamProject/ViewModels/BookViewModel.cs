using IceCreamProject.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace IceCreamProject.ViewModels
{
    public class BookViewModel
    {
        public int BookId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title must not exceed 200 characters.")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(5000, ErrorMessage = "The Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0.")]
        public decimal Price { get; set; }

        [Display(Name = "Upload Image")]
        public IFormFile? ImageUrl { get; set; }

        public string? ImagePath { get; set; }
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        public int? CategoryId { get; set; }

        public Category? Category { get; set; }

        // Dropdown list for categories
        public IEnumerable<SelectListItem>? Categories { get; set; }

        // List of recipes associated with this book
        public List<RecipeViewModel>? Recipes { get; set; }
    }
}
