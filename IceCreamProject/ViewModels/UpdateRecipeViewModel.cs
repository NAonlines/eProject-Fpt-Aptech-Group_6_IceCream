    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Http;

    namespace IceCreamProject.ViewModels
    {
        public class UpdateRecipeViewModel
        {
            [Required]
            public int RecipeId { get; set; } 

            [Required(ErrorMessage = "Recipe Name is required.")]
            [Display(Name = "Recipe Name")]
            public string Name { get; set; } 

            [Required(ErrorMessage = "Ingredients are required.")]
            [Display(Name = "Ingredients")]
            public string Ingredients { get; set; } 

            [Display(Name = "Upload New Image")]
            public IFormFile? ImageUrl { get; set; } 

            public string? ImagePath { get; set; }

        }
    }
