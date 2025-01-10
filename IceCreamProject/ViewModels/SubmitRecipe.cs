using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

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
    }
}
