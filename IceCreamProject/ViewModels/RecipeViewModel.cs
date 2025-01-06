using IceCreamProject.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace IceCreamProject.ViewModels
{
    public class RecipeViewModel
    {
        public int? RecipeId { get; set; }

        [Required(ErrorMessage = "Recipe Name is required.")]
        [Display(Name = "Recipe Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Ingredients are required.")]
        [Display(Name = "Ingredients")]
        public string Ingredients { get; set; }

        [Display(Name = "Image")]
        public IFormFile? ImageUrl { get; set; }

        public string? ImagePath { get; set; }

        public bool IsApproved { get; set; } = false;

        [Required(ErrorMessage = "Category is required.")]
        [Display(Name = "Category")]
        public int? CategoryId { get; set; }

        public List<SelectListItem>? Categories { get; set; }

        [Required(ErrorMessage = "Book is required.")]
        [Display(Name = "Book")]
        public int? BookId { get; set; }

        public List<SelectListItem>? Books { get; set; }

        public string? CreatedById { get; set; }

        public string? CreatedByName { get; set; }
    }
}
