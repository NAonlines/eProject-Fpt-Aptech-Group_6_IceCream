using IceCreamProject.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IceCreamProject.ViewModels
{
    public class SubmitRecipeViewModel
    {
        [Required(ErrorMessage = "Recipe Name is required.")]
        [Display(Name = "Recipe Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Ingredients are required.")]
        [Display(Name = "Ingredients")]
        public string Ingredients { get; set; }

        [Display(Name = "Image")]
        public IFormFile? ImageUrl { get; set; }

        [Display(Name = "Category")]
        public int? SelectedCategoryId { get; set; }

        public List<Category>? AvailableCategories { get; set; }
    }
}
