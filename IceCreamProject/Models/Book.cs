using IceCreamProject.Models;

namespace IceCreamProject.Models;
public class Book
{
    public int BookId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; }
    public int? CategoryId { get; set; }
    public Category? Category { get; set; } // Navigation property (nullable)
    public Recipe Recipe { get; set; } // Navigation property for Recipe
}
