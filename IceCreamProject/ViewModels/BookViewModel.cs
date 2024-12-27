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

		[StringLength(1000, ErrorMessage = "Description must not exceed 1000 characters.")]
		public string? Description { get; set; }

		[Required(ErrorMessage = "Price is required.")]
		[Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0.")]
		public decimal Price { get; set; }

		// For file upload
		[Display(Name = "Upload Image")]
		public IFormFile? ImageUrl { get; set; }


		// To store the path of the uploaded image
		public string? ImagePath { get; set; }

		[Required(ErrorMessage = "Category is required.")]
		public int? CategoryId { get; set; }

		// Optional: To include category details
		public Category? Category { get; set; }

		// Dropdown list for categories
		public IEnumerable<SelectListItem>? Categories { get; set; }

		// Recipe ID or reference
		public int? Recipe { get; set; }
	}
}
