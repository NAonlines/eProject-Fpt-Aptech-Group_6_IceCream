using IceCreamProject.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace IceCreamProject.ViewModels
{
    public class BookViewModel
    {
        public int BookId { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title must not exceed 200 characters.")]


        public string? Title { get; set; }

        [StringLength(1000, ErrorMessage = "Description must not exceed 1000 characters.")]
        public string? Description { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0.")]
        public decimal Price { get; set; }

        //Image address
        [Display(Name = "Name image")]
        public IFormFile? ImageUrl { get; set; }

        public string? ImagePath { get; set; }  //address image location

        public int? CategoryId { get; set; }

        public Category? Category { get; set; }

        public IEnumerable<SelectListItem>? Categories { get; set; }

        public int? Recipe { get; set; } 

    }
}
